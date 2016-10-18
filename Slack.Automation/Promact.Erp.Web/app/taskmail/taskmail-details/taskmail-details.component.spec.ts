declare let describe, it, beforeEach, expect;
import { async, inject, TestBed, ComponentFixture } from '@angular/core/testing';
import { provide, Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from '@angular/router';
import { TaskMailDetailsComponent } from './taskmail-details.component';
import { TaskService } from '../taskmail.service';
import { TestConnection } from "../../shared/mock/test.connection";
import { MockTaskMailService } from '../../shared/mock/mock.taskmailReport.service';
import { Observable } from 'rxjs/Observable';
import { DatePipe } from '@angular/common';
import { LoaderService } from '../../shared/loader.service';
import {StringConstant} from '../../shared/stringConstant';

describe('TaskMail Details Tests', () => {
    let taskMailDetailsComponent: TaskMailDetailsComponent;
    let stringConstant: StringConstant
    let router: ActivatedRoute;
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
                provide(ActivatedRoute, { useClass: MockActivatedRoute }),
                provide(TestConnection, { useClass: TestConnection }),
                provide(StringConstant, { useClass: StringConstant }),
                provide(TaskService, { useClass: MockTaskMailService }),
                provide(Router, { useClass: MockRouter }),
                provide(DatePipe, { useClass: MockDatePipe }),
                provide(LoaderService, { useClass: MockLoaderService }),
            ]
        });
    });

    beforeEach(inject([ActivatedRoute, Router, TaskService, LoaderService, StringConstant],
        (mockRouter: ActivatedRoute, router: Router, taskService: TaskService, loader: LoaderService, stringConstant: StringConstant) => {
            taskMailDetailsComponent = new TaskMailDetailsComponent(mockRouter, router, taskService, stringConstant,loader );
    }));

    it("should be defined", () => {
        expect(taskMailDetailsComponent).toBeDefined();
    });

    it('Shows details of task mail report for an employee on initialization', () => {
        taskMailDetailsComponent.getTaskMailDetails();
        expect(taskMailDetailsComponent.taskMailUser.length).toBe(1);
    });

    it('Shows details of task mail report for an employee on Privious Date', () => {
        taskMailDetailsComponent.getTaskMailPrevious("test", "1", "Admin", "10-09-2016");
        expect(taskMailDetailsComponent.taskMailUser.length).toBe(1);
    });

    it('Shows details of task mail report for an employee on Next Date', () => {
        taskMailDetailsComponent.getTaskMailNext("test", "1", "Admin", "10-09-2016");
        expect(taskMailDetailsComponent.taskMailUser.length).toBe(1);
    });

    it('Shows details of task mail report for an employee on Selected Date', () => {
        taskMailDetailsComponent.getTaskMailForSelectedDate("test", "1", "Admin", "10-09-2016","10-09-2016");
        expect(taskMailDetailsComponent.taskMailUser.length).toBe(1);
    });
})
