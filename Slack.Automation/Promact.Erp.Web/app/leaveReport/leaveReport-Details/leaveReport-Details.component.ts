import { Component, OnInit } from '@angular/core';
import { LeaveReportDetail } from './leaveReport-Details.model';
import { LeaveReportService } from '../leaveReport.service';
import { ActivatedRoute } from '@angular/router';
import { StringConstant } from '../../shared/stringConstant';
import { LoaderService } from '../../shared/loader.service';

declare let jsPDF;

@Component({
    templateUrl: './app/leaveReport/leaveReport-Details/leaveReport-Details.html',
})

export class LeaveReportDetailsComponent implements OnInit {
    leaveReportDetail: LeaveReportDetail[] = [];
    errorMessage: string;
    Id: string;
    noDetails: string;

    constructor(private leaveReportService: LeaveReportService, private route: ActivatedRoute, private stringConstant: StringConstant, private loader: LoaderService) { }

    ngOnInit() {        
        this.getLeaveReportDetail();       
    }

    getLeaveReportDetail() {
        this.loader.loader = true;
        this.route.params.subscribe(params => this.Id = params[this.stringConstant.paramsId]);
        this.leaveReportService.getLeaveReportDetail(this.Id)
            .subscribe(
            leaveReportDetail => {
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