
import { TestConnection } from "./test.connection";
import { Injectable } from '@angular/core';
import { ResponseOptions, Response } from "@angular/http";
import { LeaveReport } from '../leaveReport/leaveReport-List/leaveReport-List.model';
import { LeaveReportDetail } from '../leaveReport/leaveReport-Details/leaveReport-Details.model';

@Injectable()

export class MockLeaveReportService {

    constructor(private connection: TestConnection) {  }

    getLeaveReports() {
        let mockLeaveReports = new Array<MockLeaveReport>();
        let mockLeaveReport = new MockLeaveReport();
        mockLeaveReport.EmployeeId = "abc";
        mockLeaveReport.EmployeeUserName = "abc@abc.com";
        mockLeaveReport.EmployeeName = "abc";
        mockLeaveReport.TotalSickLeave = 7;
        mockLeaveReport.TotalCasualLeave = 14;
        mockLeaveReports.push(mockLeaveReport)
        let connection = this.getMockResponse("leaveReport", mockLeaveReports);
        return connection;
    }

    getLeaveReportDetail(Id: string) {
        let mockLeaveReportDetails = new Array<MockLeaveReportDetails>();
        let mockLeaveReportDetail = new MockLeaveReportDetails();
        if (Id === "abc") {
            mockLeaveReportDetail.EmployeeUserName = "abc@abc.com";
            mockLeaveReportDetail.EmployeeName = "abc";
            mockLeaveReportDetail.LeaveFrom = "1/1/16";
            mockLeaveReportDetails.push(mockLeaveReportDetail)
        }
        let connection = this.getMockResponse("leaveReportDetails/" +Id, mockLeaveReportDetails);
        return connection;
    }


    getMockResponse(api: string, mockBody: string  | Array<LeaveReport> | Array<LeaveReportDetail>) {
        let connection = this.connection.mockConnection(api);
        let response = new Response(new ResponseOptions({ body: mockBody }));

        //sends mock response to connection
        connection.mockRespond(response.json());
        return connection.response;
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

