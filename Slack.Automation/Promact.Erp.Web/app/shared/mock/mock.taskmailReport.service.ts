import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { TaskMailDetailsModel } from '../../taskmail/taskmaildetails.model';
import { TaskMailModel } from '../../taskmail/taskmail.model';

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
    }

}

class MockTaskmailModel extends TaskMailModel {
    constructor() {
        super();
    }
}

class MockmailModel extends TaskMailDetailsModel {
    constructor() {
        super();
    }
}

