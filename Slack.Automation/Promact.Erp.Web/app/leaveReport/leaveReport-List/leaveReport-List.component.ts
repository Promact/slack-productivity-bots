import { Component, OnInit } from '@angular/core';
import { LeaveReport } from './leaveReport-List.model';
import { LeaveReportService } from '../leaveReport.service';
import { Router } from '@angular/router';
import { StringConstant } from '../../shared/stringConstant';

declare let jsPDF;


@Component({
    templateUrl: './app/leaveReport/leaveReport-List/leaveReport-List.html',
})

export class LeaveReportListComponent implements OnInit {
    leaveReports: LeaveReport[] = [];
    errorMessage: string;
    private EmployeeName: string;
    private Role: string;
    noLeaves: string;

    constructor(private leaveReportService: LeaveReportService, private router: Router, private stringConstant: StringConstant) { }

    ngOnInit() {
        this.getLeaveReports();
    }

    getLeaveReports() {
        this.leaveReportService.getLeaveReports()
            .subscribe(
            leaveReports => {
                this.leaveReports = leaveReports;
                if (leaveReports.length !== 0) {
                    return leaveReports;
                }
                else {
                    this.noLeaves = this.stringConstant.noLeaves;
                    return this.noLeaves;
                }                    
            },
            error => this.errorMessage = <string>error
        );
    }

    leaveDetails(employeeId: string) {
        let link = [this.stringConstant.detail, employeeId];
        this.router.navigate(link);
    }

    exportDataToPdf() {
        let columns = this.stringConstant.listColumns;
        let rows = [];
        for (let key in this.leaveReports) {
            rows.push([
                this.leaveReports[key].EmployeeName,
                this.leaveReports[key].EmployeeUserName,
                this.leaveReports[key].Role,
                this.leaveReports[key].TotalSickLeave,
                this.leaveReports[key].TotalCasualLeave,
                this.leaveReports[key].UtilisedCasualLeave,
                this.leaveReports[key].BalanceCasualLeave,
                this.leaveReports[key].UtilisedSickLeave,
                this.leaveReports[key].BalanceSickLeave
                ]);
        };

        let doc = new jsPDF(this.stringConstant.portrait, this.stringConstant.unit, this.stringConstant.format);

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