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
var filter_pipe_1 = require('../filter.pipe');
var LeaveReportComponent = (function () {
    function LeaveReportComponent(leaveReportService, router) {
        this.leaveReportService = leaveReportService;
        this.router = router;
        this.mode = 'Observable';
    }
    LeaveReportComponent.prototype.ngOnInit = function () {
        this.getLeaveReports();
    };
    LeaveReportComponent.prototype.getLeaveReports = function () {
        var _this = this;
        this.leaveReportService.getLeaveReports()
            .subscribe(function (leaveReports) { return _this.leaveReports = leaveReports; }, function (error) { return _this.errorMessage = error; });
    };
    LeaveReportComponent.prototype.leaveDetails = function (employeeName) {
        var link = ['/detail', employeeName];
        this.router.navigate(link);
    };
    LeaveReportComponent.prototype.exportData = function () {
        var columns = ["Employee Name", "Total Sick Leave", "Total Casual Leave", "Utilised Casual Leave", "Balance Casual Leave"];
        var rows = [];
        for (var key in this.leaveReports) {
            rows.push([
                this.leaveReports[key].EmployeeName,
                this.leaveReports[key].TotalSickLeave,
                this.leaveReports[key].TotalCasualLeave,
                this.leaveReports[key].UtilisedCasualLeave,
                this.leaveReports[key].BalanceCasualLeave
            ]);
        }
        ;
        var doc = new jsPDF('p', 'pt');
        doc.autoTable(columns, rows, {
            styles: { fillColor: [100, 255, 255] },
            columnStyles: {
                id: { fillColor: 255 }
            },
        });
        doc.save('table.pdf');
    };
    LeaveReportComponent = __decorate([
        core_1.Component({
            selector: '',
            templateUrl: './app/leaveReport/leaveReportList/leaveReportList.html',
            pipes: [filter_pipe_1.FilterPipe]
        }), 
        __metadata('design:paramtypes', [leaveReport_service_1.LeaveReportService, router_1.Router])
    ], LeaveReportComponent);
    return LeaveReportComponent;
}());
exports.LeaveReportComponent = LeaveReportComponent;
//# sourceMappingURL=leaveReport.js.map