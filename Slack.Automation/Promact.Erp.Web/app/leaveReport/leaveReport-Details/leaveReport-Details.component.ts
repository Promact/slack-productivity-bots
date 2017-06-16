import { Component, OnInit } from '@angular/core';
import { LeaveReportDetail } from './leaveReport-Details.model';
import { LeaveReportService } from '../leaveReport.service';
import { ActivatedRoute } from '@angular/router';
import { StringConstant } from '../../shared/stringConstant';
import { LoaderService } from '../../shared/loader.service';
import { JsonToPdfService } from '../../shared/jsontopdf.service';

@Component({
    moduleId: module.id,
    templateUrl: 'leaveReport-Details.html',
})

export class LeaveReportDetailsComponent implements OnInit {
    leaveReportDetail: LeaveReportDetail[] = [];
    errorMessage: string;
    Id: string;
    noDetails: string;

    constructor(private leaveReportService: LeaveReportService, private route: ActivatedRoute, private stringConstant: StringConstant, private loader: LoaderService, private jsPDF: JsonToPdfService) { }

    ngOnInit() {
        this.getLeaveReportDetail();
    }

    getLeaveReportDetail() {
        this.loader.loader = true;
        this.route.params.subscribe(params => this.Id = params[this.stringConstant.paramsId]);
        this.leaveReportService.getLeaveReportDetail(this.Id)
            .subscribe(
            leaveReportDetail => {
                if (this.leaveReportDetail !== null && this.leaveReportDetail !== undefined) {
                    for (let index = 0; index < leaveReportDetail.length; index++) {
                        let leave = leaveReportDetail[index];
                        leave.Reason = this.replaceSpecialCharacter(leave.Reason);
                    }
                }
                this.leaveReportDetail = leaveReportDetail;
                if (leaveReportDetail.length !== 0) {
                    this.loader.loader = false;
                    return leaveReportDetail;
                }
                else {
                    this.noDetails = this.stringConstant.noDetails;
                    this.loader.loader = false;
                    return this.noDetails;
                }
            },
            error => this.errorMessage = <string>error
            );
    }

    goBack() {
        window.history.back();
    }

    exportDataToPdf() {
        let columns = this.stringConstant.detailColumns;
        let rows = [];
        for (let key in this.leaveReportDetail) {
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
        this.jsPDF.exportJsonToPdf(columns, rows);
    }

    replaceSpecialCharacter(text: string) {
        return text.replace('&amp;', '&').replace('&lt;', '<').replace('&gt;', '>');
    }
}