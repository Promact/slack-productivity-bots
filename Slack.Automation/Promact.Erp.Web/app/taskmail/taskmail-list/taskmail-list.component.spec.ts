
declare var describe, it, beforeEach, expect;
import { async, inject, TestBed, ComponentFixture, fakeAsync } from '@angular/core/testing';
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
import { Md2Toast, Md2SelectChange } from 'md2';
import { MockSelectChange } from '../../shared/mock/mock.md2selectchange';
import { MailSettingModule } from '../../shared/MailSetting/mailsetting.module';

let promise: TestBed;

let stringConstant = new StringConstant();
describe('Task Mail Report List Tests', () => {
    const routes: Routes = [];
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [RouterLinkStubDirective],
            imports: [TaskMailModule, MailSettingModule, RouterModule.forRoot(routes, { useHash: true })
            ],
            providers: [
                { provide: TaskService, useClass: MockTaskMailService },
                { provide: StringConstant, useClass: StringConstant },
                { provide: Router, useClass: MockRouter },
                { provide: LoaderService, useClass: LoaderService },
                { provide: Md2Toast, useClass: Md2Toast },
                { provide: Md2SelectChange, useClass: MockSelectChange }
            ]
        }).compileComponents();
    }));

    it("should be defined", fakeAsync(() => {
        let fixture = TestBed.createComponent(TaskMailListComponent);
        let taskMailListComponent = fixture.componentInstance;
        expect(taskMailListComponent).toBeDefined();
    }));

    it('Shows list of TaskReports on initialization', fakeAsync(()  => {
            let fixture = TestBed.createComponent(TaskMailListComponent); //Create instance of component            
            let taskMailListComponent = fixture.componentInstance;
            let result = taskMailListComponent.ngOnInit();
            expect(taskMailListComponent.taskMailUsers.length).toBe(1);
    }));

    it('Shows list of taskmailReport on initialization for Admin', fakeAsync(() => {
        let fixture = TestBed.createComponent(TaskMailListComponent);
        let taskMailListComponent = fixture.componentInstance;
        taskMailListComponent.getListOfEmployee();
        expect(taskMailListComponent.taskMailUsers.length).toBe(1);
    }));

    it('Shows list of taskmailReport on get list of employee', fakeAsync(() => {
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
    }));
    it('Shows list of taskmailReport on initialization for TeamLeader', fakeAsync(() => {
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
    }));

    it('should be rediration to taskmail details', fakeAsync(() => {
        let fixture = TestBed.createComponent(TaskMailListComponent);
        let taskMailListComponent = fixture.componentInstance;
        let router = fixture.debugElement.injector.get(Router);
        spyOn(router, "navigate");
        taskMailListComponent.taskmailDetails(stringConstant.userId, stringConstant.userName, stringConstant.userEmail);
        expect(router.navigate).toHaveBeenCalled();
    }));
});