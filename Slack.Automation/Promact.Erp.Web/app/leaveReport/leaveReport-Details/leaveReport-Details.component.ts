import { Component } from '@angular/core';
import { LeaveReportDetail } from './leaveReport-Details.model';
import { LeaveReportService } from '../leaveReport.service';
import { ActivatedRoute } from '@angular/router';
declare let jsPDF: any;

@Component({
    templateUrl: './app/leaveReport/leaveReport-Details/leaveReport-Details.html',
})

export class LeaveReportDetailsComponent {
    leaveReportDetail: LeaveReportDetail[] = [];
    errorMessage: string;
    sub: any;
    Id: any;

    constructor(private leaveReportService: LeaveReportService, private route: ActivatedRoute) {
    }

    ngOnInit() {
        this.getLeaveReportDetail();
    }

    getLeaveReportDetail() {        
        this.sub = this.route.params.subscribe(params => this.Id = params['id']);
        this.leaveReportService.getLeaveReportDetail(this.Id)
            .subscribe(
            leaveReportDetail => this.leaveReportDetail = leaveReportDetail,
            error => this.errorMessage = <any>error            
            );
    }

    goBack() {
        window.history.back();
    }

    exportDataToPdf() {
        var columns = ["Employee Name", "Employee Username", "Leave From", "Start Day", "Leave Upto", "End Day","Reason"];
        var rows: any = [];
        for (var key in this.leaveReportDetail) {
            rows.push([
                this.leaveReportDetail[key].EmployeeName,
                this.leaveReportDetail[key].EmployeeUserName,
                this.leaveReportDetail[key].LeaveFrom,
                this.leaveReportDetail[key].StartDay,
                this.leaveReportDetail[key].LeaveUpto,
                this.leaveReportDetail[key].EndDay,
                this.leaveReportDetail[key].Reason
            ]);
        };

        var doc = new jsPDF('p', 'pt', 'a4');

        doc.autoTable(columns, rows, {
            styles: {
                theme: 'plain',
                overflow: 'linebreak',
                pageBreak: 'auto',
                tableWidth: 'auto',
            },
        });
        doc.save('ReportDetail.pdf');
    }    
}