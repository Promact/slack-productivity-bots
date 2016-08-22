import { Component } from '@angular/core';
import { LeaveReportDetail } from '../leaveReportDetails.model';
import { LeaveReportService } from '../leaveReport.service';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: '',
    templateUrl: './app/leaveReport/leaveReportDetails/leaveReportDetails.html',
})

export class LeaveReportDetailsComponent {
    leaveReportDetail: LeaveReportDetail[];
    mode = 'Observable';
    errorMessage: string;
    sub: any;
    Id: any;

    constructor(private leaveReportService: LeaveReportService, private route: ActivatedRoute) { }

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
}