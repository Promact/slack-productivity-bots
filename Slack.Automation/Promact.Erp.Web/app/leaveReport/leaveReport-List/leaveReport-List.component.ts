import { Component, OnInit } from '@angular/core';
import { LeaveReport } from './leaveReport-List.model';
import { LeaveReportService } from '../leaveReport.service';
import { Router } from '@angular/router';
import { FilterPipe } from '../filter.pipe';
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

    constructor(private leaveReportService: LeaveReportService, private router: Router) { }

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

    leaveDetails(employeeId: string) {
        let link = ['/detail', employeeId];
        this.router.navigate(link);
    }

    exportDataToPdf() {
        var columns = ["Employee Name", "Employee Username","Total Sick Leave", "Total Casual Leave", "Utilised Casual Leave", "Balance Casual Leave"];
        var rows: any = [];
        for (var key in this.leaveReports) {
            rows.push([
                this.leaveReports[key].EmployeeName,
                this.leaveReports[key].EmployeeUserName,
                this.leaveReports[key].TotalSickLeave,
                this.leaveReports[key].TotalCasualLeave,
                this.leaveReports[key].UtilisedCasualLeave,
                this.leaveReports[key].BalanceCasualLeave
                ]);
        };

        var doc = new jsPDF('p','pt','a4');

        doc.autoTable(columns, rows, {
            styles: {
                theme: 'plain',
                overflow: 'linebreak',
                pageBreak: 'auto', 
                tableWidth: 'auto',
            },
        });
        doc.save('Report.pdf');
    }    
}