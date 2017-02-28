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
import { MockRouter } from '../../shared/mock/mock.router';
import { ActivatedRouteStub } from '../../shared/mock/mock.activatedroute';
import { TaskMailModel } from '../../taskmail/taskmail.model';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
let promise: TestBed;

let stringConstant = new StringConstant();
describe('Task Mail Report List Tests', () => {
   const routes: Routes = [];
   beforeEach(async(() => {
        this.promise = TestBed.configureTestingModule({
            declarations: [RouterLinkStubDirective], 
            imports: [TaskMailModule, RouterModule.forRoot(routes, { useHash: true })  
            ],
            providers: [
                { provide: ActivatedRoute, useClass: ActivatedRouteStub },
                { provide: TaskService, useClass: MockTaskMailService },
                { provide: StringConstant, useClass: StringConstant },
                { provide: Router, useClass: MockRouter },
                { provide: LoaderService, useClass: LoaderService }
            ]
        }).compileComponents();
    }));

    it("should be defined", () => {
        let fixture = TestBed.createComponent(TaskMailListComponent);
        let taskMailListComponent = fixture.componentInstance;
        expect(taskMailListComponent).toBeDefined();
    });

    it('Shows list of TaskReports on initialization', () => done => { 
        this.promise.then(() => {
            let fixture = TestBed.createComponent(TaskMailListComponent); //Create instance of component            
            let taskMailListComponent = fixture.componentInstance;
            let result = taskMailListComponent.ngOnInit();
            expect(taskMailListComponent.taskMailUsers.length).toBe(1);
            done();
        });
    });

    it('Shows list of taskmailReport on initialization for Admin', () => {
        let fixture = TestBed.createComponent(TaskMailListComponent);
        let taskMailListComponent = fixture.componentInstance;
        taskMailListComponent.getListOfEmployee();
        expect(taskMailListComponent.taskMailUsers.length).toBe(1);
    });

    it('Shows list of taskmailReport on initialization', () => {
        let mockTaskMailModels = new Array<TaskMailModel>();
        let mockTaskMailModel = new TaskMailModel();
        mockTaskMailModel.UserName = stringConstant.userName;
        mockTaskMailModel.UserRole = stringConstant.userName;
        mockTaskMailModels.push(mockTaskMailModel);
        let fixture = TestBed.createComponent(TaskMailListComponent);
        let taskMailListComponent = fixture.componentInstance;
        let taskService = fixture.debugElement.injector.get(TaskService);
        let router = fixture.debugElement.injector.get(Router);
        spyOn(taskService, "getListOfEmployee").and.returnValue(new BehaviorSubject(mockTaskMailModels).asObservable());
        spyOn(router, "navigate");
        taskMailListComponent.getListOfEmployee();
        expect(router.navigate).toHaveBeenCalled();
    });
    it('Shows list of taskmailReport on initialization for TeamLeader', () => {
        let mockTaskMailModels = new Array<TaskMailModel>();
        let mockTaskMailModel = new TaskMailModel();
        mockTaskMailModel.UserName = stringConstant.userName;
        mockTaskMailModel.UserRole = stringConstant.RoleTeamLeader;
        mockTaskMailModels.push(mockTaskMailModel);
        let fixture = TestBed.createComponent(TaskMailListComponent);
        let taskMailListComponent = fixture.componentInstance;
        let taskService = fixture.debugElement.injector.get(TaskService);
        let router = fixture.debugElement.injector.get(Router);
        spyOn(taskService, "getListOfEmployee").and.returnValue(new BehaviorSubject(mockTaskMailModels).asObservable());
        spyOn(router, "navigate");
        taskMailListComponent.getListOfEmployee();
        expect(router.navigate).toHaveBeenCalled();
    });

    it('should be rediration to taskmail details', () => {
        let fixture = TestBed.createComponent(TaskMailListComponent);
        let taskMailListComponent = fixture.componentInstance;
        let router = fixture.debugElement.injector.get(Router);
        spyOn(router, "navigate");
        taskMailListComponent.taskmailDetails(stringConstant.userId, stringConstant.userName, stringConstant.userEmail);
        expect(router.navigate).toHaveBeenCalled();
    });
});





