import { Component } from '@angular/core';
import { LeaveReportDetail } from './leaveReport-Details.model';
import { LeaveReportService } from '../leaveReport.service';
import { ActivatedRoute } from '@angular/router';
import { StringConstant } from '../../shared/stringConstant';

declare let jsPDF: any;

@Component({
    templateUrl: './app/leaveReport/leaveReport-Details/leaveReport-Details.html',
})

export class LeaveReportDetailsComponent {
    leaveReportDetail: LeaveReportDetail[] = [];
    errorMessage: string;
    sub: any;
    Id: any;
    noDetails: string;

    constructor(private leaveReportService: LeaveReportService, private route: ActivatedRoute, private stringConstant: StringConstant) { }

    ngOnInit() {
        this.getLeaveReportDetail();
    }

    getLeaveReportDetail() {
        this.sub = this.route.params.subscribe(params => this.Id = params[this.stringConstant.paramsId]);
        this.leaveReportService.getLeaveReportDetail(this.Id)
            .subscribe(
            leaveReportDetail => {
                this.leaveReportDetail = leaveReportDetail
                if (leaveReportDetail.length != 0) {
                    return leaveReportDetail;
                }
                else {
                    this.noDetails = this.stringConstant.noDetails;
                    return this.noDetails;
                }
            },
            error => this.errorMessage = <any>error            
            );
    }

    goBack() {
        window.history.back();
    }

    exportDataToPdf() {
        var columns = this.stringConstant.detailColumns;
        var rows: any = [];
        for (var key in this.leaveReportDetail) {
            rows.push([
                this.leaveReportDetail[key].EmployeeName,
                this.leaveReportDetail[key].EmployeeUserName,
                this.leaveReportDetail[key].Type,
                this.leaveReportDetail[key].LeaveFrom,
                this.leaveReportDetail[key].StartDay,
                this.leaveReportDetail[key].LeaveUpto,
                this.leaveReportDetail[key].EndDay,
                this.leaveReportDetail[key].Reason
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