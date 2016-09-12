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
var FilterPipe = (function () {
    function FilterPipe() {
    }
    FilterPipe.prototype.transform = function (leaveReports, employeeName, utilisedCasualLeave, role) {
        if (leaveReports === null) {
            return null;
        }
        if ((utilisedCasualLeave == 0 || utilisedCasualLeave == undefined) && (employeeName === undefined || employeeName === "" || employeeName === null) && (role === undefined || role === "" || role === null)) {
            return leaveReports;
        }
        if ((employeeName === "" || employeeName === undefined) && (role === "" || role === undefined)) {
            return leaveReports.filter(function (y) { return y.UtilisedCasualLeave == utilisedCasualLeave; });
        }
        if ((employeeName === "" || employeeName === undefined) && utilisedCasualLeave == 0) {
            return leaveReports.filter(function (y) { return y.Role.toLowerCase().startsWith(role.toLowerCase()); });
        }
        if (utilisedCasualLeave == 0 && (role == "" || role === undefined)) {
            return leaveReports.filter(function (y) { return y.EmployeeName.toLowerCase().startsWith(employeeName.toLowerCase()); });
        }
        if (utilisedCasualLeave == 0) {
            return leaveReports.filter(function (y) { return y.EmployeeName.toLowerCase().startsWith(employeeName.toLowerCase()) || y.Role.toLowerCase().startsWith(role.toLowerCase()); });
        }
        if (employeeName === "") {
            return leaveReports.filter(function (y) { return y.Role.toLowerCase().startsWith(role.toLowerCase()) || y.UtilisedCasualLeave == utilisedCasualLeave; });
        }
        if (role === "") {
            return leaveReports.filter(function (y) { return y.EmployeeName.toLowerCase().startsWith(employeeName.toLowerCase()) || y.UtilisedCasualLeave == utilisedCasualLeave; });
        }
        return leaveReports.filter(function (y) { return y.EmployeeName.toLowerCase().startsWith(employeeName.toLowerCase()) || y.UtilisedCasualLeave == utilisedCasualLeave || y.Role.startsWith(role); });
    };
    FilterPipe = __decorate([
        core_1.Pipe({ name: 'filter' }),
        core_1.Injectable(), 
        __metadata('design:paramtypes', [])
    ], FilterPipe);
    return FilterPipe;
}());
exports.FilterPipe = FilterPipe;
//# sourceMappingURL=filter.pipe.js.map