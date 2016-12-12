import { Injectable } from '@angular/core';


@Injectable()
export class StringConstant {
    constructor() { }

    leaveReport = "leaveReport";
    leaveReportDetails = "leaveReportDetails/";
    serverError = 'Server error';
    listColumns = ["Employee Name", "Employee Username", "Role", "Total Sick Leave", "Total Casual Leave", "Utilised Casual Leave", "Balance Casual Leave"];
    theme = 'plain';
    overflow = 'linebreak';
    pageBreak = 'auto';
    tableWidth = 'auto';
    save = 'Report.pdf';
    portrait = 'p';
    unit = 'pt';
    format = 'a4';
    detail = '/detail';
    detailColumns = ["Employee Name", "Employee Username", "Leave From", "Start Day", "Leave Upto", "End Day", "Reason"];
    paramsId = 'id';
    scrum = "api/project";
    slash = "/";
    defaultDate = '1-01-01';
    notAvailableComment = 'Not Available';
    RoleAdmin = "Admin";
    RoleTeamLeader = "TeamLeader";
    taskList = "/task";
    dateDefaultFormat = "yyyy-MM-dd";
    dateFormat = "dd-MM-yyyy";
    taskDetails = "task/taskdetail";
    noProjectToDisplay = "No projects to display";
    userRole= "role";
    userName= "name";
    createdOns = "createdOn";
    taskDetailsUrl = "/user/";
    selectedDate = "SelectedDate";
    taskMaiUrl = "api/taskreport";
    next = "Next";
    previous = "Previous";
    pageType = "PageType";
}