declare let describe, it, beforeEach, expect;
import { async, inject, TestBed, ComponentFixture } from '@angular/core/testing';
import { provide, Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from '@angular/router';
import { TaskMailDetailsComponent } from './taskmail-details.component';
import { TaskService } from '../taskmail.service';
import { TestConnection } from "../../mock/test.connection";
import { MockTaskMailService } from '../../mock/mock.taskmailReport.service';
import { Observable } from 'rxjs/Observable';
import { DatePipe } from '@angular/common';
import { SpinnerService} from '../../spinner.service';

describe('TaskMail Details Tests', () => {
    let taskMailDetailsComponent: TaskMailDetailsComponent;
    let router: ActivatedRoute;
    class MockActivatedRoute extends ActivatedRoute {
        constructor() {
            super();
            this.params = Observable.of({ UserId: "1", UserRole: "Admin", UserName:"test" });
        }
    }
    class MockRouter { }
    class MockDatePipe { }
    class MockSpinnerService { }

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [
                provide(ActivatedRoute, { useClass: MockActivatedRoute }),
                provide(TestConnection, { useClass: TestConnection }),
                provide(TaskService, { useClass: MockTaskMailService }),
                provide(Router, { useClass: MockRouter }),
                provide(DatePipe, { useClass: MockDatePipe }),
                provide(SpinnerService, { useClass: MockSpinnerService }),
            ]
        });
    });

    beforeEach(inject([ActivatedRoute, Router, TaskService], (mockRouter: ActivatedRoute, router: Router, taskService: TaskService, spinner: SpinnerService) => {
        taskMailDetailsComponent = new TaskMailDetailsComponent(mockRouter, router, taskService, spinner);
    }));

    it("should be defined", () => {
        expect(taskMailDetailsComponent).toBeDefined();
    });

    it('Shows details of task mail report for an employee on initialization', () => {
        taskMailDetailsComponent.getTaskMailDetails();
        expect(taskMailDetailsComponent.taskMailUser.length).toBe(1);
    });

    it('Shows details of task mail report for an employee on Privious Date', () => {
        taskMailDetailsComponent.getTaskMailPrevious("test","1", "Admin", "10-09-2016");
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
