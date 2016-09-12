declare let describe, it, beforeEach, expect;
import { async, inject, TestBed, ComponentFixture } from '@angular/core/testing';
import { provide, Component, OnInit } from "@angular/core";
import { Router } from '@angular/router';
import { TaskMailListComponent } from './taskmail-list.component';
import { TaskService } from '../taskmail.service';
import { TestConnection } from "../../mock/test.connection";
import { MockTaskMailService } from '../../mock/mock.taskmailReport.service';

describe('TaskMailReport Tests', () => {
    let taskMailListComponent: TaskMailListComponent;
    let router: Router;
    class MockRouter { }

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [
                provide(Router, { useClass: MockRouter }),
                provide(TestConnection, { useClass: TestConnection }),
                provide(TaskService, { useClass: MockTaskMailService }),
            ]
        });
    });

    beforeEach(inject([TaskService, Router], (taskService: TaskService, mockRouter: Router) => {
        router = mockRouter;
        taskMailListComponent = new TaskMailListComponent(router, taskService);
    }));


    it('Shows list of taskmailReport on initialization', () => {
        taskMailListComponent.ngOnInit();
        expect(taskMailListComponent.taskMails.length).toBe(1);
    });

})





