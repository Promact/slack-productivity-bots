import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { LeaveReport } from '../../leaveReport/leaveReport-List/leaveReport-List.model';
import { LeaveReportDetail } from '../../leaveReport/leaveReport-Details/leaveReport-Details.model';
import { StringConstant } from '../stringConstant';


@Injectable()

export class MockLeaveReportService {

    getLeaveReports() {
        let mockLeaveReports = new Array<MockLeaveReport>();
        let mockLeaveReport = new MockLeaveReport();
        mockLeaveReport.EmployeeId = "abc";
        mockLeaveReport.EmployeeUserName = "abc@abc.com";
        mockLeaveReport.EmployeeName = "abc";
        mockLeaveReport.TotalSickLeave = 7;
        mockLeaveReport.TotalCasualLeave = 14;
        mockLeaveReports.push(mockLeaveReport);
        return new BehaviorSubject(mockLeaveReports).asObservable();
        //let connection = this.getMockResponse(this.stringConstant.leaveReport, mockLeaveReports);
        //return connection;
    }

    getLeaveReportDetail(Id: string) {
        let mockLeaveReportDetails = new Array<MockLeaveReportDetails>();
        let mockLeaveReportDetail = new MockLeaveReportDetails();
        if (Id === "abc") {
            mockLeaveReportDetail.EmployeeUserName = "abc@abc.com";
            mockLeaveReportDetail.EmployeeName = "abc";
            mockLeaveReportDetail.LeaveFrom = "1/1/16";
            mockLeaveReportDetails.push(mockLeaveReportDetail);
        }
        return new BehaviorSubject(mockLeaveReportDetails).asObservable();
        //let connection = this.getMockResponse(this.stringConstant.leaveReportDetails +Id, mockLeaveReportDetails);
        //return connection;
    }
    
}

class MockLeaveReport extends LeaveReport {
      constructor() {
        super();
        }
}

class MockLeaveReportDetails extends LeaveReportDetail {
    constructor() {
        super();
    }
}



