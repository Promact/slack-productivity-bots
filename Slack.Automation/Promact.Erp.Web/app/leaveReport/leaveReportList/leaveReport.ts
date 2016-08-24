import { Component, OnInit } from '@angular/core';
import { LeaveReport } from '../leaveReport.model';
import { LeaveReportService } from '../leaveReport.service';
import { Router } from '@angular/router';
import { FilterPipe } from '../filter.pipe';
//declare let jsPDF: any;
//declare let get: any;


@Component({
    templateUrl: './app/leaveReport/leaveReportList/leaveReportList.html',
    pipes: [FilterPipe]
})

export class LeaveReportComponent implements OnInit {
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
    }

    leaveDetails(employeeId: string) {
        let link = ['/detail', employeeId];
        this.router.navigate(link);
    }

    //exportData() {

    //    var columns = ["Employee Name", "Total Sick Leave", "Total Casual Leave", "Utilised Casual Leave", "Balance Casual Leave"];
    //    var rows: any = [];
    //    for (var key in this.leaveReports) {
    //        rows.push([
    //            this.leaveReports[key].EmployeeName,
    //            this.leaveReports[key].TotalSickLeave,
    //            this.leaveReports[key].TotalCasualLeave,
    //            this.leaveReports[key].UtilisedCasualLeave,
    //            this.leaveReports[key].BalanceCasualLeave
    //            ]);
    //    };

    //    var doc = new jsPDF('p', 'pt');

    //    doc.autoTable(columns, rows, {
    //        styles: { fillColor: [100, 255, 255] },
    //        columnStyles: {
    //            id: { fillColor: 255 }
    //        },
    //    });
    //    doc.save('table.pdf');

    //}

}