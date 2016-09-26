declare let describe, it, beforeEach, expect;
import { async, inject, TestBed, ComponentFixture } from '@angular/core/testing';
import { provide, Component, OnInit } from "@angular/core";
import { ActivatedRoute,Router } from '@angular/router';
import { TaskMailListComponent } from './taskmail-list.component';
import { TaskService } from '../taskmail.service';
import { TestConnection } from "../../shared/mock/test.connection";
import { MockTaskMailService } from '../../shared/mock/mock.taskmailReport.service';
import { Observable } from 'rxjs/Observable';
import { SpinnerService} from '../../shared/spinner.service';
import {StringConstant} from '../../shared/stringConstant';


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
    class MockSpinnerService { }

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [
                provide(Router, { useClass: MockRouter }),
                provide(TestConnection, { useClass: TestConnection }),
                provide(TaskService, { useClass: MockTaskMailService }),
                provide(SpinnerService, { useClass: MockSpinnerService }),
                provide(StringConstant, { useClass: StringConstant }),
            ]
        });
    });

    beforeEach(inject([TaskService, Router, SpinnerService, StringConstant], (taskService: TaskService, router: Router, spinner: SpinnerService, stringConstant: StringConstant) => {

        taskMailListComponent = new TaskMailListComponent(router, taskService, spinner, stringConstant);
    }));


    it('Shows list of taskmailReport on initialization', () => {
        taskMailListComponent.getListOfEmployee();
        expect(taskMailListComponent.listOfUsers.length).toBe(1);
    });

})





