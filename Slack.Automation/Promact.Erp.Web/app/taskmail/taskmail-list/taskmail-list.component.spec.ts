declare var describe, it, beforeEach, expect;
import { async, inject, TestBed, ComponentFixture } from '@angular/core/testing';
import { Provider } from "@angular/core";
import { Observable } from 'rxjs/Observable';
import { Router, ActivatedRoute, RouterModule, Routes } from '@angular/router';
import { RouterLinkStubDirective } from '../../shared/mock/mock.routerLink';
import { TaskService } from '../taskmail.service';
import { SpinnerService } from '../../shared/spinner.service';
import { StringConstant } from '../../shared/stringConstant';
import { MockTaskMailService } from '../../shared/mock/mock.taskmailReport.service';
import { Observable } from 'rxjs/Observable';
import { LoaderService } from '../../shared/loader.service';
import {StringConstant} from '../../shared/stringConstant';
import { TaskMailListComponent } from './taskmail-list.component';
import { TaskMailModule } from '../../taskmail/taskMail.module';

let promise: TestBed;


describe('Task Mail Report List Tests', () => {
    class MockRouter { }
    class MockDatePipe { }
    class MockSpinnerService { }
    const routes: Routes = [];
    class MockActivatedRoute extends ActivatedRoute {
        constructor() {
            super();
            this.params = Observable.of({ id: "1" });
        }
    }
    class MockRouter { }
    class MockLoaderService { }

    beforeEach(async(() => {
        this.promise = TestBed.configureTestingModule({
            declarations: [RouterLinkStubDirective], //Declaration of mock routerLink used on page.
            imports: [TaskMailModule, RouterModule.forRoot(routes, { useHash: true }) //Set LocationStrategy for component. 
            ],
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

    it("should be defined", () => {
        let fixture = TestBed.createComponent(TaskMailListComponent);           
        let taskMailListComponent = fixture.componentInstance;
        expect(taskMailListComponent).toBeDefined();
    });

    it('Shows list of taskmailReport on initialization', () => {
        let fixture = TestBed.createComponent(TaskMailListComponent);   
        let taskMailListComponent = fixture.componentInstance;
        taskMailListComponent.getListOfEmployee();
        expect(taskMailListComponent.listOfUsers.length).toBe(1);
    });
});







