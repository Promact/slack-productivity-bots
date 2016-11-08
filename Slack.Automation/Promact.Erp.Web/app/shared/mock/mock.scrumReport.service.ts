import { TestConnection } from './test.connection';
import { Injectable } from '@angular/core';
import { ResponseOptions, Response } from '@angular/http';
import { ScrumDetails } from '../../ScrumReport/scrumProject-Details/scrumProject-Details.model';
import { EmployeeScrumAnswers } from '../../ScrumReport/scrumProject-Details/scrumProject-EmployeeScrumDetails.model';
import { ScrumProject } from '../../ScrumReport/scrumProject-List/scrumProject-List.model';
import { StringConstant } from '../stringConstant';

@Injectable()

export class MockScrumReportService {

    constructor(private connection: TestConnection, private stringConstant: StringConstant) { }

    getScrumProjects() {
        let mockScrumProjects = new Array<ScrumProject>();
        let mockScrumProject = new ScrumProject();
        mockScrumProject.Name = "aaaa";
        mockScrumProjects.push(mockScrumProject);
        let connection = this.getMockResponse(this.stringConstant.scrumProjects, mockScrumProjects);
        return connection;
    }    

    getScrumDetails(Id: number, Date: any) {
        let mockScrumDetails = new ScrumDetails();
        let mockEmployeeScrumAnswers = new Array<EmployeeScrumAnswers>();
        let mockEmployeeScrumAnswer = new EmployeeScrumAnswers();
        mockEmployeeScrumAnswer.Answer1 = "abc";
        mockEmployeeScrumAnswer.Answer2 = "abc2";
        mockEmployeeScrumAnswer.Answer3 = "no";
        mockEmployeeScrumAnswer.EmployeeName = "xyz";
        mockEmployeeScrumAnswers.push(mockEmployeeScrumAnswer);
        if (Id == 123){
            mockScrumDetails.ScrumDate = Date;
            mockScrumDetails.ProjectCreationDate = "1/1/16";
            mockScrumDetails.EmployeeScrumAnswers = mockEmployeeScrumAnswers;
        }

        let connection = this.getMockResponse(this.stringConstant.scrumDetails + Id + this.stringConstant.slash + Date, mockScrumDetails);
        return connection;
    }

    getMockResponse(api: string, mockBody: Array<ScrumProject> | ScrumDetails) {
        let connection = this.connection.mockConnection(api);
        let response = new Response(new ResponseOptions({ body: mockBody }));

        connection.mockRespond(response.json());
        return connection.response;
    }
}

    class MockEmployeeScrumAnswers extends EmployeeScrumAnswers {
        constructor() {
            super();
        }
    }

    class MockScrumDetails extends ScrumDetails {
        constructor() {
            super();
        }
    }

    class MockScrumProject extends ScrumProject {
        constructor() {
            super();
        }
    } 

