declare var describe, it, beforeEach, expect;
import { async, inject, TestBed, ComponentFixture } from '@angular/core/testing';
import { Provider } from "@angular/core";
import { Router, ActivatedRoute, RouterModule, Routes } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { RouterLinkStubDirective } from '../../shared/mock/mock.routerLink';
import { TaskMailModule } from '../../taskmail/taskMail.module';
import { MockTaskMailService } from '../../shared/mock/mock.taskmailReport.service';
import { TaskService } from '../taskmail.service';
import { DatePipe } from '@angular/common';
import { LoaderService } from '../../shared/loader.service';
import {StringConstant} from '../../shared/stringConstant';
import { TaskMailDetailsComponent } from './taskmail-details.component';

let promise: TestBed;

describe('LeaveReport Detials Tests', () => {
    class MockRouter { }
    class MockDatePipe { }
    class MockLoaderService { }
    const routes: Routes = [];
    class MockActivatedRoute extends ActivatedRoute {
        constructor() {
            super();
            this.params = Observable.of({ UserId: "1", UserRole: "Admin", UserName: "test" });
        }
    }
    class MockRouter { }
    class MockDatePipe { }
    class MockLoaderService { }

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [
                { provide: ActivatedRoute, useClass: MockActivatedRoute },
                { provide: TaskService, useClass: MockTaskMailService },
                { provide: StringConstant, useClass: StringConstant },
                { provide: Router, useClass: MockRouter },
                { provide: DatePipe, useClass: MockDatePipe },
                { provide: LoaderService, useClass: MockLoaderService }
            ]
        });
    });

    beforeEach(inject([ActivatedRoute, Router, TaskService, LoaderService, StringConstant],
        (mockRouter: ActivatedRoute, router: Router, taskService: TaskService, loader: LoaderService, stringConstant: StringConstant) => {
            taskMailDetailsComponent = new TaskMailDetailsComponent(mockRouter, router, taskService, stringConstant,loader );
    }));

    it("should be defined", () => {
        let fixture = TestBed.createComponent(TaskMailDetailsComponent); //Create instance of component            
        let taskMailDetailsComponent = fixture.componentInstance;
        expect(taskMailDetailsComponent).toBeDefined();
    });

    it('Shows details of task mail report for an employee on initialization', () => {
        let fixture = TestBed.createComponent(TaskMailDetailsComponent); //Create instance of component            
        let taskMailDetailsComponent = fixture.componentInstance;
        taskMailDetailsComponent.getTaskMailDetails();
        expect(taskMailDetailsComponent.taskMailUser.length).toBe(1);
    });

    it('Shows details of task mail report for an employee on Privious Date', () => {
        let fixture = TestBed.createComponent(TaskMailDetailsComponent); //Create instance of component            
        let taskMailDetailsComponent = fixture.componentInstance;
        taskMailDetailsComponent.getTaskMailPrevious("test", "1", "Admin", "10-09-2016");
        expect(taskMailDetailsComponent.taskMailUser.length).toBe(1);
    });

    it('Shows details of task mail report for an employee on Next Date', () => {
        let fixture = TestBed.createComponent(TaskMailDetailsComponent); //Create instance of component            
        let taskMailDetailsComponent = fixture.componentInstance;
        taskMailDetailsComponent.getTaskMailNext("test", "1", "Admin", "10-09-2016");
        expect(taskMailDetailsComponent.taskMailUser.length).toBe(1);
    });

    it('Shows details of task mail report for an employee on Selected Date', () => {
        let fixture = TestBed.createComponent(TaskMailDetailsComponent); //Create instance of component            
        let taskMailDetailsComponent = fixture.componentInstance;
        taskMailDetailsComponent.getTaskMailForSelectedDate("test", "1", "Admin", "10-09-2016","10-09-2016");
        expect(taskMailDetailsComponent.taskMailUser.length).toBe(1);
    });
});






