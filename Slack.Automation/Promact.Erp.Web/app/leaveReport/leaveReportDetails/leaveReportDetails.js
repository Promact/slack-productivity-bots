"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var core_1 = require('@angular/core');
var leaveReport_service_1 = require('../leaveReport.service');
var router_1 = require('@angular/router');
var LeaveReportDetailsComponent = (function () {
    function LeaveReportDetailsComponent(leaveReportService, route) {
        this.leaveReportService = leaveReportService;
        this.route = route;
        this.leaveReportDetail = [];
    }
    LeaveReportDetailsComponent.prototype.ngOnInit = function () {
        this.leaveReportDetail = [];
        this.getLeaveReportDetail();
    };
    LeaveReportDetailsComponent.prototype.getLeaveReportDetail = function () {
        var _this = this;
        this.sub = this.route.params.subscribe(function (params) { return _this.Id = params['id']; });
        this.leaveReportService.getLeaveReportDetail(this.Id)
            .subscribe(function (leaveReportDetail) { return _this.leaveReportDetail = leaveReportDetail; }, function (error) { return _this.errorMessage = error; });
    };
    LeaveReportDetailsComponent.prototype.goBack = function () {
        window.history.back();
    };
    LeaveReportDetailsComponent.prototype.exportDataToPdf = function () {
        var columns = ["Employee Name", "Employee Username", "Leave From", "Start Day", "Leave Upto", "End Day", "Reason"];
        var rows = [];
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
        }
        ;
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
    };
    LeaveReportDetailsComponent = __decorate([
        core_1.Component({
            templateUrl: './app/leaveReport/leaveReportDetails/leaveReportDetails.html',
        }), 
        __metadata('design:paramtypes', [leaveReport_service_1.LeaveReportService, router_1.ActivatedRoute])
    ], LeaveReportDetailsComponent);
    return LeaveReportDetailsComponent;
}());
exports.LeaveReportDetailsComponent = LeaveReportDetailsComponent;
//# sourceMappingURL=leaveReportDetails.js.map