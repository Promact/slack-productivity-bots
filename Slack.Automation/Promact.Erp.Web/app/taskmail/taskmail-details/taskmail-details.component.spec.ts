declare var describe, it, beforeEach, expect;
import { async, inject, TestBed, ComponentFixture } from '@angular/core/testing';
import { Provider } from "@angular/core";
import { Router, ActivatedRoute, RouterModule, Routes } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { RouterLinkStubDirective } from '../../shared/mock/mock.routerLink';
import { TaskMailModule } from '../../taskmail/taskMail.module';
import { MockTaskMailService } from '../../shared/mock/mock.taskmailReport.service';
import { TaskService } from '../taskmail.service';
import { DatePipe } from '@angular/common';
import { LoaderService } from '../../shared/loader.service';
import { StringConstant } from '../../shared/stringConstant';
import { TaskMailDetailsComponent } from './taskmail-details.component';
import { MockRouter } from '../../shared/mock/mock.router';
import { ActivatedRouteStub } from '../../shared/mock/mock.activatedroute';
import { Md2SelectChange } from 'md2';
import { AppModule } from '../../app.module';
import { TaskMailModel } from '../../taskmail/taskmail.model';
import { TaskMailDetailsModel } from '../../taskmail/taskmaildetails.model';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';

//let promise: TestBed;
//let stringConstant = new StringConstant();

//describe('TaskMail Detials Tests', () => {
//    const routes: Routes = [];
//    class MockMd2Select { }
//    beforeEach(async(() => {
//        this.promise = TestBed.configureTestingModule({
//            declarations: [RouterLinkStubDirective],
//            imports: [AppModule, RouterModule.forRoot(routes, { useHash: true }) 
//            ],
//            providers: [
//                { provide: ActivatedRoute, useClass: ActivatedRouteStub },
//                { provide: TaskService, useClass: MockTaskMailService },
//                { provide: StringConstant, useClass: StringConstant },
//                { provide: Router, useClass: MockRouter },
//                { provide: DatePipe, useClass: DatePipe },
//                { provide: LoaderService, useClass: LoaderService },
//                { provide: Md2SelectChange, useClass: MockMd2Select }
//            ]
//        }).compileComponents();
//    }));

//    it("should be defined", fakeAsync(() => {
//            let fixture = TestBed.createComponent(TaskMailDetailsComponent);
//            let taskMailDetailsComponent = fixture.componentInstance;
//            tick();
//            expect(taskMailDetailsComponent).toBeDefined();
//        }));


    it('Shows list of taskMail Details', () => {
        let fixture = TestBed.createComponent(TaskMailDetailsComponent); //Create instance of component  
        let activatedRoute = fixture.debugElement.injector.get(ActivatedRoute);
        activatedRoute.testParams = { UserId: stringConstant.userId, UserRole: stringConstant.RoleAdmin, UserName: stringConstant.userName };          
        let taskMailDetailsComponent = fixture.componentInstance;
        let result = taskMailDetailsComponent.ngOnInit();
        expect(taskMailDetailsComponent.taskMail.length).toBe(1);
    });

     it('Shows details of task mail report for an employee on initialization For Admin', () => {
        let fixture = TestBed.createComponent(TaskMailDetailsComponent); 
        let activatedRoute = fixture.debugElement.injector.get(ActivatedRoute);
        activatedRoute.testParams = { UserId: stringConstant.userId, UserRole: stringConstant.RoleAdmin, UserName: stringConstant.userName };
        let taskMailDetailsComponent = fixture.componentInstance;
        taskMailDetailsComponent.getTaskMailDetails();
        expect(taskMailDetailsComponent.taskMail.length).toBe(1);
    });

     
    it('Shows details of task mail report for an employee on initialization For Teamleader', () => {
        let fixture = TestBed.createComponent(TaskMailDetailsComponent);
        let activatedRoute = fixture.debugElement.injector.get(ActivatedRoute);
        activatedRoute.testParams = { UserId: stringConstant.userId, UserRole: stringConstant.RoleTeamLeader, UserName: stringConstant.userName };
        let taskMailDetailsComponent = fixture.componentInstance;
        taskMailDetailsComponent.getTaskMailDetails();
        expect(taskMailDetailsComponent.taskMail.length).toBe(1);
    });

    it('Shows details of task mail report for an employee on Privious Date For Admin', () => {
        let fixture = TestBed.createComponent(TaskMailDetailsComponent);       
        let taskMailDetailsComponent = fixture.componentInstance;
        taskMailDetailsComponent.getTaskMailPrevious(stringConstant.userName, stringConstant.userId, stringConstant.RoleAdmin, stringConstant.createdOn);
        expect(taskMailDetailsComponent.taskMail.length).toBe(1);
    });

    it('Shows details of task mail report for an employee on Privious Date For Teamleader', () => {
        let fixture = TestBed.createComponent(TaskMailDetailsComponent);
        let taskMailDetailsComponent = fixture.componentInstance;
        taskMailDetailsComponent.getTaskMailPrevious(stringConstant.userName, stringConstant.userId, stringConstant.RoleTeamLeader, stringConstant.createdOn);
        expect(taskMailDetailsComponent.taskMail.length).toBe(1);
    });

    it('Shows details of task mail report for an employee on Next Date for Admin', () => {
        let fixture = TestBed.createComponent(TaskMailDetailsComponent);          
        let taskMailDetailsComponent = fixture.componentInstance;
        taskMailDetailsComponent.getTaskMailNext(stringConstant.userName, stringConstant.userId, stringConstant.RoleAdmin, stringConstant.createdOn);
        expect(taskMailDetailsComponent.taskMail.length).toBe(1);
    });

    it('Shows details of task mail report for an employee on Next Date for Teamleader', () => {
        let fixture = TestBed.createComponent(TaskMailDetailsComponent);
        let taskMailDetailsComponent = fixture.componentInstance;
        taskMailDetailsComponent.getTaskMailNext(stringConstant.userName, stringConstant.userId, stringConstant.RoleTeamLeader, stringConstant.createdOn);
        expect(taskMailDetailsComponent.taskMail.length).toBe(1);
    });

    it('Shows details of task mail report for an employee on Selected Date for Admin', () => {
        let fixture = TestBed.createComponent(TaskMailDetailsComponent);            
        let taskMailDetailsComponent = fixture.componentInstance;
        taskMailDetailsComponent.getTaskMailForSelectedDate(stringConstant.userName, stringConstant.userId, stringConstant.RoleAdmin, stringConstant.createdOn, stringConstant.createdOn);
        expect(taskMailDetailsComponent.taskMail.length).toBe(1);
    });

    it('Shows details of task mail report for an employee on Selected Date for Teamleader', () => {
        let fixture = TestBed.createComponent(TaskMailDetailsComponent);
        let taskMailDetailsComponent = fixture.componentInstance;
        taskMailDetailsComponent.getTaskMailForSelectedDate(stringConstant.userName, stringConstant.userId, stringConstant.RoleTeamLeader, stringConstant.createdOn, stringConstant.createdOn);
        expect(taskMailDetailsComponent.taskMail.length).toBe(1);
    });

    it('should be rediration to taskmail details', () => {
        let fixture = TestBed.createComponent(TaskMailDetailsComponent);
        let taskMailDetailsComponent = fixture.componentInstance;
        let router = fixture.debugElement.injector.get(Router);
        spyOn(router, "navigate");
        taskMailDetailsComponent.getTaskMailList();
        expect(router.navigate).toHaveBeenCalled();
    });
});
