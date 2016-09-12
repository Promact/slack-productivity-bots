
import { TestConnection } from "./test.connection";
import { Injectable } from '@angular/core';
import { ResponseOptions, Response } from "@angular/http";
import { taskmailModel } from '../taskmail/taskmail.model';


@Injectable()

export class MockTaskMailService {

    constructor(private connection: TestConnection) { }

    getTaskMailReport() {
        let mockTaskMailModels = new Array<MockTaskmailModel>();
        let mockTaskMailModel = new MockTaskmailModel();
        mockTaskMailModel.Id = 1;
        mockTaskMailModel.CreatedOn = new Date("01-01-2016");
        mockTaskMailModel.UserName = "test";
        mockTaskMailModels.push(mockTaskMailModel);
        let connection = this.getMockResponse("taskMailReport", mockTaskMailModels);
        return connection;
    }

    getTaskMailDetailsReport(Id: string) {
        let mockTaskmailModels = new Array<MockTaskmailModel>();
        let mockTaskmailModel = new MockTaskmailModel();
        if (Id.toString() === "1") {
            mockTaskmailModel.Description = "test Description";
            mockTaskmailModel.Comment = "test Comment";
            mockTaskmailModel.Hours = 1.5;
            mockTaskmailModel.Status = 1;
            mockTaskmailModels.push(mockTaskmailModel);
        }
        let connection = this.getMockResponse("taskMailDetailsReport/" + Id, mockTaskmailModels);
        return connection;
    }


    getMockResponse(api: string, mockBody: string | Array<taskmailModel>) {
        let connection = this.connection.mockConnection(api);
        let response = new Response(new ResponseOptions({ body: mockBody }));

        //sends mock response to connection
        connection.mockRespond(response.json());
        return connection.response;
    }
}

class MockTaskmailModel extends taskmailModel {
    constructor() {
        super();
    }
}



