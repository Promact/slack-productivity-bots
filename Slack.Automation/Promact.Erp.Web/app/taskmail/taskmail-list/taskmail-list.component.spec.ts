declare let describe, it, beforeEach, expect;
import { async, inject, TestBed, ComponentFixture } from '@angular/core/testing';
import { provide, Component, OnInit } from "@angular/core";
import { ActivatedRoute,Router } from '@angular/router';
import { TaskMailListComponent } from './taskmail-list.component';
import { TaskService } from '../taskmail.service';
import { TestConnection } from "../../shared/mock/test.connection";
import { MockTaskMailService } from '../../shared/mock/mock.taskmailReport.service';
import { Observable } from 'rxjs/Observable';
import { LoaderService } from '../../shared/loader.service';
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
    class MockLoaderService { }

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [
                provide(Router, { useClass: MockRouter }),
                provide(TestConnection, { useClass: TestConnection }),
                provide(TaskService, { useClass: MockTaskMailService }),
                provide(LoaderService, { useClass: MockLoaderService }),
                provide(StringConstant, { useClass: StringConstant }),
            ]
        });
    });

    beforeEach(inject([TaskService, Router, LoaderService, StringConstant], (taskService: TaskService, router: Router, loader: LoaderService, stringConstant: StringConstant) => {

        taskMailListComponent = new TaskMailListComponent(router, taskService, stringConstant,loader );
    }));


    it('Shows list of taskmailReport on initialization', () => {
        taskMailListComponent.getListOfEmployee();
        expect(taskMailListComponent.listOfUsers.length).toBe(1);
    });

})





