import { Component, OnInit } from '@angular/core';
import { LeaveReport } from './leaveReport-List.model';
import { LeaveReportService } from '../leaveReport.service';
import { Router } from '@angular/router';
import { FilterPipe } from '../filter.pipe';
import { StringConstant } from '../../shared/stringConstant';
declare let jsPDF: any;


@Component({
    templateUrl: './app/leaveReport/leaveReport-List/leaveReport-List.html',
    pipes: [FilterPipe]
})

export class LeaveReportListComponent implements OnInit {
    leaveReports: LeaveReport[] = [];
    errorMessage: string;
    private EmployeeName: string;
    private UtilisedCasualLeave: number;
    private Role: string;

    constructor(private leaveReportService: LeaveReportService, private router: Router, private stringConstant: StringConstant) { }

    ngOnInit() {
        this.getLeaveReports();
    }

    getLeaveReports() {
        this.leaveReportService.getLeaveReports()
            .subscribe(
            leaveReports => this.leaveReports = leaveReports,               
            error => this.errorMessage = <any>error
        );
        return this.leaveReports;
    }

    //leaveDetails(employeeId: string) {
    //    let link = ['/detail', employeeId];
    //    this.router.navigate(link);
    //}

    exportDataToPdf() {
        var columns = this.stringConstant.listColumns;
        var rows: any = [];
        for (var key in this.leaveReports) {
            rows.push([
                this.leaveReports[key].EmployeeName,
                this.leaveReports[key].EmployeeUserName,
                this.leaveReports[key].Role,
                this.leaveReports[key].TotalSickLeave,
                this.leaveReports[key].TotalCasualLeave,
                this.leaveReports[key].UtilisedCasualLeave,
                this.leaveReports[key].BalanceCasualLeave
                ]);
        };

        var doc = new jsPDF(this.stringConstant.portrait, this.stringConstant.unit, this.stringConstant.format);

        doc.autoTable(columns, rows, {
            styles: {
                theme: this.stringConstant.theme,
                overflow: this.stringConstant.overflow,
                pageBreak: this.stringConstant.pageBreak,
                tableWidth: this.stringConstant.tableWidth,
            },
        });
        doc.save(this.stringConstant.save);
    }    
}