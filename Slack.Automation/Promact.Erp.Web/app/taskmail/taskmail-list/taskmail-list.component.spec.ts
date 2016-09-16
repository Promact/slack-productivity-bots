declare let describe, it, beforeEach, expect;
import { async, inject, TestBed, ComponentFixture } from '@angular/core/testing';
import { provide, Component, OnInit } from "@angular/core";
import { ActivatedRoute,Router } from '@angular/router';
import { TaskMailListComponent } from './taskmail-list.component';
import { TaskService } from '../taskmail.service';
import { TestConnection } from "../../mock/test.connection";
import { MockTaskMailService } from '../../mock/mock.taskmailReport.service';
import { Observable } from 'rxjs/Observable';
describe('TaskMailReport Tests', () => {
    let taskMailListComponent: TaskMailListComponent;
    let router: Router;
    let Activatedrouter: ActivatedRoute;
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
                provide(Router, { useClass: MockRouter }),
                provide(TestConnection, { useClass: TestConnection }),
                provide(TaskService, { useClass: MockTaskMailService }),
                //provide(ActivatedRoute, { useClass: MockActivatedRoute }),
            ]
        });
    });

    beforeEach(inject([TaskService, Router], ( taskService: TaskService, router: Router) => {
        
        taskMailListComponent = new TaskMailListComponent(router, taskService);
    }));


    it('Shows list of taskmailReport on initialization', () => {
        taskMailListComponent.getListOfEmployee();
        expect(taskMailListComponent.listOfUsers.length).toBe(1);
    });

})





