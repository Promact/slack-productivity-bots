declare let describe, it, beforeEach, expect;
import { async, inject, TestBed, ComponentFixture } from '@angular/core/testing';
import { provide, Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from '@angular/router';
import { TaskMailDetailsComponent } from './taskmail-details.component';
import { TaskService } from '../taskmail.service';
import { TestConnection } from "../../mock/test.connection";
import { MockTaskMailService } from '../../mock/mock.taskmailReport.service';
import { Observable } from 'rxjs/Observable';

describe('TaskMail Details Tests', () => {
    let taskMailDetailsComponent: TaskMailDetailsComponent;
    let router: ActivatedRoute;
    class MockActivatedRoute extends ActivatedRoute {
        constructor() {
            super();
            this.params = Observable.of({ id: "1" });
        }
    }
    class MockRouter { }
    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [
                provide(ActivatedRoute, { useClass: MockActivatedRoute }),
                provide(TestConnection, { useClass: TestConnection }),
                provide(TaskService, { useClass: MockTaskMailService }),
                provide(Router, { useClass: MockRouter }),
            ]
        });
    });

    beforeEach(inject([ActivatedRoute, Router, TaskService], (mockRouter: ActivatedRoute, router: Router, taskService: TaskService) => {
        //router = mockRouter;
        taskMailDetailsComponent = new TaskMailDetailsComponent(mockRouter, router, taskService);
    }));


    //it('Shows details of task mail report for an employee on initialization', () => {
    //    taskMailDetailsComponent.ngOnInit();
    //    expect(taskMailDetailsComponent.taskMails.length).toBe(1);
    //});
})
