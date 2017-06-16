import { TestConnection } from './test.connection';
import { Injectable } from '@angular/core';
import { ResponseOptions, Response } from '@angular/http';
import { ScrumDetails } from '../../ScrumReport/scrumProject-Details/scrumProject-Details.model';
import { EmployeeScrumAnswers } from '../../ScrumReport/scrumProject-Details/scrumProject-EmployeeScrumDetails.model';
import { ScrumProject } from '../../ScrumReport/scrumProject-List/scrumProject-List.model';
import { StringConstant } from '../stringConstant';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';


@Injectable()

export class MockScrumReportService {

    constructor(private stringConstant: StringConstant) { }

    getScrumProjects() {
        let mockScrumProjects = new Array<ScrumProject>();
        let mockScrumProject = new ScrumProject();
        mockScrumProject.Name = this.stringConstant.projectName;
        mockScrumProjects.push(mockScrumProject);
        return new BehaviorSubject(mockScrumProjects).asObservable();
    }

    getScrumDetails(id: number, date: string) {
        let mockScrumDetails = new ScrumDetails();
        let mockEmployeeScrumAnswers = new Array<EmployeeScrumAnswers>();
        let mockEmployeeScrumAnswer = new EmployeeScrumAnswers();
        mockEmployeeScrumAnswer.Answer1 = [this.stringConstant.Answer1];
        mockEmployeeScrumAnswer.Answer2 = [this.stringConstant.Answer2];
        mockEmployeeScrumAnswer.Answer3 = [this.stringConstant.Answer3];
        mockEmployeeScrumAnswer.EmployeeName = this.stringConstant.EmployeeName;
        mockEmployeeScrumAnswers.push(mockEmployeeScrumAnswer);
        if (id === 123) {
            mockScrumDetails.ScrumDate = date;
            mockScrumDetails.ProjectCreationDate = this.stringConstant.ProjectCreationDate;
            mockScrumDetails.EmployeeScrumAnswers = mockEmployeeScrumAnswers;
        }
        return new BehaviorSubject(mockScrumDetails).asObservable();
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
