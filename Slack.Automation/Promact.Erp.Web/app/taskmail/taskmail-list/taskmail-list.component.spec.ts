declare var describe, it, beforeEach, expect;
import { async, inject, TestBed, ComponentFixture } from '@angular/core/testing';
import { Provider } from "@angular/core";
import { Observable } from 'rxjs/Observable';
import { Router, ActivatedRoute, RouterModule, Routes } from '@angular/router';
import { RouterLinkStubDirective } from '../../shared/mock/mock.routerLink';
import { TaskService } from '../taskmail.service';
import { StringConstant } from '../../shared/stringConstant';
import { MockTaskMailService } from '../../shared/mock/mock.taskmailReport.service';
import { LoaderService } from '../../shared/loader.service';
import { TaskMailListComponent } from './taskmail-list.component';
import { TaskMailModule } from '../../taskmail/taskMail.module';

let promise: TestBed;


describe('Task Mail Report List Tests', () => {
    class MockRouter { }
    class MockDatePipe { }
    class MockLoaderService { }
    const routes: Routes = [];
    class MockActivatedRoute extends ActivatedRoute {
        constructor() {
            super();
            this.params = Observable.of({ id: "1" });
        }
    }
    beforeEach(async(() => {
        this.promise = TestBed.configureTestingModule({
            declarations: [RouterLinkStubDirective], //Declaration of mock routerLink used on page.
            imports: [TaskMailModule, RouterModule.forRoot(routes, { useHash: true }) //Set LocationStrategy for component. 
            ],
            providers: [
                { provide: ActivatedRoute, useClass: MockActivatedRoute },
                { provide: TaskService, useClass: MockTaskMailService },
                { provide: StringConstant, useClass: StringConstant },
                { provide: Router, useClass: MockRouter },
                { provide: LoaderService, useClass: MockLoaderService }
            ]
        }).compileComponents();
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





