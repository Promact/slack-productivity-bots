import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { taskmailModel } from '../../taskmail/taskmail.model';
import { taskmailuserModel } from '../../taskmail/taskmailuser.model';

@Injectable()
export class MockTaskMailService {
    constructor() {

    }
    getListOfEmployee() {
        let mockTaskMailModels = new Array<MockTaskmailModel>();
        let mockTaskMailModel = new MockTaskmailModel();
        mockTaskMailModel.UserName = "test";
        mockTaskMailModel.UserRole = "Admin";
        mockTaskMailModels.push(mockTaskMailModel);
        return new BehaviorSubject(mockTaskMailModels).asObservable();
    }

    getTaskMailDetailsReport(UserId: string, UserRole: string, UserName: string) {
        let mockTaskmailModels = new Array<MockTaskmailModel>();
        let mockmailModels = new Array<MockmailModel>();
        let mockmailModel = new MockmailModel();
        mockmailModel.Comment = "test Comment";
        mockmailModel.Hours = 1;
        mockmailModel.Description = "test Description";
        mockmailModels.push(mockmailModel);
        let mockTaskmailModel = new MockTaskmailModel();
        mockTaskmailModel.UserName = UserName;
        mockTaskmailModel.UserRole = UserRole;
        mockTaskmailModel.UserId = UserId;
        mockTaskmailModel.TaskMails = mockmailModels;
        mockTaskmailModel.IsMin = new Date("10-09-2016");
        mockTaskmailModel.IsMax = new Date("10-09-2016");
        mockTaskmailModels.push(mockTaskmailModel);
        return new BehaviorSubject(mockTaskmailModels).asObservable();

        //let connection = this.getMockResponse("taskMailDetailsReport/" + UserId + "/" + UserRole + "/" + UserName, mockTaskmailModels);
        //return connection;
    }

    getTaskMailDetailsReportPreviousDate(UserId: string, UserRole: string, UserName: string, CreatedOn: string) {
        let mockTaskmailModels = new Array<MockTaskmailModel>();
        let mockmailModels = new Array<MockmailModel>();
        let mockmailModel = new MockmailModel();
        mockmailModel.Comment = "test Comment";
        mockmailModel.Hours = 1;
        mockmailModel.Description = "test Description";
        mockmailModels.push(mockmailModel);
        let mockTaskmailModel = new MockTaskmailModel();
        mockTaskmailModel.UserName = UserName;
        mockTaskmailModel.UserRole = UserRole;
        mockTaskmailModel.UserId = UserId;
        mockTaskmailModel.TaskMails = mockmailModels;
        mockTaskmailModel.IsMin = new Date("10-09-2016");
        mockTaskmailModel.IsMax = new Date("10-09-2016");
        mockTaskmailModels.push(mockTaskmailModel);
        return new BehaviorSubject(mockTaskmailModels).asObservable();
        //let connection = this.getMockResponse("taskMailDetailsReportPreviousDate/" + UserId + "/" + UserRole + "/" + UserName + "/" + CreatedOn, mockTaskmailModels);
        //return connection;
    }
    
    getTaskMailDetailsReportNextDate(UserId: string, UserRole: string, UserName: string, CreatedOn: string) {
        let mockTaskmailModels = new Array<MockTaskmailModel>();
        let mockmailModels = new Array<MockmailModel>();
        let mockmailModel = new MockmailModel();
        mockmailModel.Comment = "test Comment";
        mockmailModel.Hours = 1;
        mockmailModel.Description = "test Description";
        mockmailModels.push(mockmailModel);
        let mockTaskmailModel = new MockTaskmailModel();
        mockTaskmailModel.UserName = UserName;
        mockTaskmailModel.UserRole = UserRole;
        mockTaskmailModel.UserId = UserId;
        mockTaskmailModel.TaskMails = mockmailModels;
        mockTaskmailModel.IsMin = new Date("10-09-2016");
        mockTaskmailModel.IsMax = new Date("10-09-2016");
        mockTaskmailModels.push(mockTaskmailModel);
        return new BehaviorSubject(mockTaskmailModels).asObservable();
        //let connection = this.getMockResponse("taskMailDetailsReportNextDate/" + UserId + "/" + UserRole + "/" + UserName + "/" + CreatedOn, mockTaskmailModels);
        //return connection;
    }
    
    getTaskMailDetailsReportSelectedDate(UserId: string, UserRole: string, UserName: string, CreatedOn: string, SelectedDate: string) {
        let mockTaskmailModels = new Array<MockTaskmailModel>();
        let mockmailModels = new Array<MockmailModel>();
        let mockmailModel = new MockmailModel();
        mockmailModel.Comment = "test Comment";
        mockmailModel.Hours = 1;
        mockmailModel.Description = "test Description";
        mockmailModels.push(mockmailModel);
        let mockTaskmailModel = new MockTaskmailModel();
        mockTaskmailModel.UserName = UserName;
        mockTaskmailModel.UserRole = UserRole;
        mockTaskmailModel.UserId = UserId;
        mockTaskmailModel.TaskMails = mockmailModels;
        mockTaskmailModel.IsMin = new Date("10-09-2016");
        mockTaskmailModel.IsMax = new Date("10-09-2016");
        mockTaskmailModels.push(mockTaskmailModel);
        return new BehaviorSubject(mockTaskmailModels).asObservable();
        //let connection = this.getMockResponse("taskMailDetailsReportSelectedDate/" + UserId + "/" + UserRole + "/" + UserName + "/" + CreatedOn + "/" + SelectedDate, mockTaskmailModels);
        //return connection;
    }

}

class MockTaskmailModel extends taskmailuserModel {
    constructor() {
        super();
    }
}

class MockmailModel extends taskmailModel {
    constructor() {
        super();
    }
}












//    getTaskMailDetailsReportNextDate(UserId: string, UserRole: string, UserName: string, CreatedOn: string) {
//        let mockTaskmailModels = new Array<MockTaskmailModel>();
//        let mockmailModels = new Array<MockmailModel>();
//        let mockmailModel = new MockmailModel();
//        mockmailModel.Comment = "test Comment";
//        mockmailModel.Hours = 1;
//        mockmailModel.Description = "test Description";
//        mockmailModels.push(mockmailModel);
//        let mockTaskmailModel = new MockTaskmailModel();
//        mockTaskmailModel.UserName = UserName;
//        mockTaskmailModel.UserRole = UserRole;
//        mockTaskmailModel.UserId = UserId;
//        mockTaskmailModel.TaskMails = mockmailModels;
//        mockTaskmailModel.IsMin = new Date("10-09-2016");
//        mockTaskmailModel.IsMax = new Date("10-09-2016");
//        mockTaskmailModels.push(mockTaskmailModel);
//        let connection = this.getMockResponse("taskMailDetailsReportNextDate/" + UserId + "/" + UserRole + "/" + UserName + "/" + CreatedOn, mockTaskmailModels);
//        return connection;
//    }
//    getTaskMailDetailsReportSelectedDate(UserId: string, UserRole: string, UserName: string, CreatedOn: string, SelectedDate: string) {
//        let mockTaskmailModels = new Array<MockTaskmailModel>();
//        let mockmailModels = new Array<MockmailModel>();
//        let mockmailModel = new MockmailModel();
//        mockmailModel.Comment = "test Comment";
//        mockmailModel.Hours = 1;
//        mockmailModel.Description = "test Description";
//        mockmailModels.push(mockmailModel);
//        let mockTaskmailModel = new MockTaskmailModel();
//        mockTaskmailModel.UserName = UserName;
//        mockTaskmailModel.UserRole = UserRole;
//        mockTaskmailModel.UserId = UserId;
//        mockTaskmailModel.TaskMails = mockmailModels;
//        mockTaskmailModel.IsMin = new Date("10-09-2016");
//        mockTaskmailModel.IsMax = new Date("10-09-2016");
//        mockTaskmailModels.push(mockTaskmailModel);
//        let connection = this.getMockResponse("taskMailDetailsReportSelectedDate/" + UserId + "/" + UserRole + "/" + UserName + "/" + CreatedOn + "/" + SelectedDate, mockTaskmailModels);
//        return connection;
//    }
//    getMockResponse(api: string, mockBody: string | Array<taskmailuserModel>) {
//        let connection = this.connection.mockConnection(api);
//        let response = new Response(new ResponseOptions({ body: mockBody }));

//        //sends mock response to connection
//        connection.mockRespond(response.json());
//        return connection.response;
//    }
//}




