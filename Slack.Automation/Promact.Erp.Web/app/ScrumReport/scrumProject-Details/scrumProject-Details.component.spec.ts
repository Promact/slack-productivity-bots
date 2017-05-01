declare var describe, it, beforeEach, expect, spyOn;
import { async, inject, TestBed, ComponentFixture, tick, fakeAsync } from '@angular/core/testing';
import { Provider } from "@angular/core";
import { Router, ActivatedRoute, RouterModule, Routes } from '@angular/router';
import { RouterLinkStubDirective } from '../../shared/mock/mock.routerLink';
import { ScrumModule } from '../scrumReport.module';
import { ScrumProjectDetailComponent } from './scrumProject-Details.component';
import { ScrumReportService } from '../scrumReport.service';
import { TestConnection } from "../../shared/mock/test.connection";
import { MockScrumReportService } from '../../shared/mock/mock.scrumReport.service';
import { Observable } from 'rxjs/Observable';
import { StringConstant } from '../../shared/stringConstant';
import { LoaderService } from '../../shared/loader.service';
import { AppModule } from '../../app.module';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { ScrumDetails } from '../../ScrumReport/scrumProject-Details/scrumProject-Details.model';
import { EmployeeScrumAnswers } from '../../ScrumReport/scrumProject-Details/scrumProject-EmployeeScrumDetails.model';
import { MailSettingModule } from '../../shared/MailSetting/mailsetting.module';
import { MockRouter } from '../../shared/mock/mock.router';
import { ActivatedRouteStub } from '../../shared/mock/mock.activatedroute';

let promise: TestBed;
let stringConstant = new StringConstant();

describe('ScrumReport Tests', () => {
    const routes: Routes = [];
    beforeEach(async(() => {
        this.promise = TestBed.configureTestingModule({
            declarations: [RouterLinkStubDirective],
            imports: [ScrumModule,MailSettingModule, RouterModule.forRoot(routes, { useHash: true })],
            providers: [
                { provide: ActivatedRoute, useClass: ActivatedRouteStub },
                { provide: ScrumReportService, useClass: MockScrumReportService },
                { provide: StringConstant, useClass: StringConstant },
                { provide: LoaderService, useClass: LoaderService },
                { provide: Router, useClass: MockRouter }
            ]
        }).compileComponents();
    }));
    
    it('Shows scrum answers of employees in a project on initialization', fakeAsync(() => {
            let fixture = TestBed.createComponent(ScrumProjectDetailComponent);
            let activatedRoute = fixture.debugElement.injector.get(ActivatedRoute);
            activatedRoute.params = Observable.of({ Id: stringConstant.userId });
            
            let scrumProjectDetailsComponent = fixture.componentInstance;
            let scrumService = fixture.debugElement.injector.get(ScrumReportService);
            let mockScrumDetails = new ScrumDetails();
            let mockEmployeeScrumAnswers = new Array<EmployeeScrumAnswers>();
            let mockEmployeeScrumAnswer = new EmployeeScrumAnswers();
            mockEmployeeScrumAnswer.Answer1 = stringConstant.Answer1;
            mockEmployeeScrumAnswer.Answer2 = stringConstant.Answer2;
            mockEmployeeScrumAnswer.Answer3 = stringConstant.Answer3;
            mockEmployeeScrumAnswer.EmployeeName = stringConstant.EmployeeName;
            mockEmployeeScrumAnswers.push(mockEmployeeScrumAnswer);
            let date = new Date().toString();
            let id = 123;
            if (id === 123) {
                mockScrumDetails.ScrumDate = date;
                mockScrumDetails.ProjectCreationDate = stringConstant.ProjectCreationDate;
                mockScrumDetails.EmployeeScrumAnswers = mockEmployeeScrumAnswers;
            }
            
            spyOn(scrumService, "getScrumDetails").and.returnValue(new BehaviorSubject(mockScrumDetails).asObservable());
            let result = scrumProjectDetailsComponent.ngOnInit();
            tick();
            expect(scrumProjectDetailsComponent.employeeScrumAnswers.length).toBe(1);
    }));

    it('Shows scrum answers of employees null', fakeAsync(() => {
        let fixture = TestBed.createComponent(ScrumProjectDetailComponent);
        let activatedRoute = fixture.debugElement.injector.get(ActivatedRoute);
        activatedRoute.params = Observable.of({ Id: stringConstant.userId });

        let scrumProjectDetailsComponent = fixture.componentInstance;
        let scrumService = fixture.debugElement.injector.get(ScrumReportService);
        let mockScrumDetails = new ScrumDetails();
        let date = new Date().toString();
        let id = 123;
        if (id === 123) {
            mockScrumDetails.ScrumDate = date;
            mockScrumDetails.ProjectCreationDate = stringConstant.ProjectCreationDate;
            mockScrumDetails.EmployeeScrumAnswers = null;
        }

        spyOn(scrumService, "getScrumDetails").and.returnValue(new BehaviorSubject(mockScrumDetails).asObservable());
        let result = scrumProjectDetailsComponent.ngOnInit();
        tick();
        expect(scrumProjectDetailsComponent.projectCreationDate).toBe(stringConstant.ProjectCreationDate);
    }));


    it('Shows scrum answers of employees in a project on initialization', () => {
        let mockScrumDetails = new ScrumDetails();
        let mockEmployeeScrumAnswers = new Array<EmployeeScrumAnswers>();
        let mockEmployeeScrumAnswer = new EmployeeScrumAnswers();
        mockEmployeeScrumAnswer.Answer1 = stringConstant.Answer1;
        mockEmployeeScrumAnswer.Answer2 = stringConstant.Answer2;
        mockEmployeeScrumAnswer.Answer3 = stringConstant.Answer3;
        mockEmployeeScrumAnswer.EmployeeName = stringConstant.EmployeeName;
        mockEmployeeScrumAnswers.push(mockEmployeeScrumAnswer);
        mockScrumDetails.ScrumDate = stringConstant.ProjectCreationDate;
        mockScrumDetails.ProjectCreationDate = stringConstant.ProjectCreationDate;
        mockScrumDetails.EmployeeScrumAnswers = mockEmployeeScrumAnswers;
        let fixture = TestBed.createComponent(ScrumProjectDetailComponent);
        let activatedRoute = fixture.debugElement.injector.get(ActivatedRoute);
        activatedRoute.params = Observable.of({ Id: stringConstant.userId });

        let scrumReportService = fixture.debugElement.injector.get(ScrumReportService);
        spyOn(scrumReportService, "getScrumDetails").and.returnValue(new BehaviorSubject(mockScrumDetails).asObservable());
        let scrumProjectDetailComponent = fixture.componentInstance;
        scrumProjectDetailComponent.ngOnInit();
        expect(scrumProjectDetailComponent.employeeScrumAnswers.length).toBe(1);
    });

    it('Shows scrum answers of employees in a project on initialization', () => {
        let mockScrumDetails = new ScrumDetails();
        let mockEmployeeScrumAnswers = new Array<EmployeeScrumAnswers>();
        let mockEmployeeScrumAnswer = new EmployeeScrumAnswers();
        mockEmployeeScrumAnswer.Answer1 = stringConstant.Answer1;
        mockEmployeeScrumAnswer.Answer2 = stringConstant.Answer2;
        mockEmployeeScrumAnswer.Answer3 = stringConstant.Answer3;
        mockEmployeeScrumAnswer.EmployeeName = stringConstant.EmployeeName;
        mockEmployeeScrumAnswers.push(mockEmployeeScrumAnswer);
        mockScrumDetails.ScrumDate = stringConstant.ProjectCreationDate;
        mockScrumDetails.ProjectCreationDate = stringConstant.ProjectCreationDate;
        mockScrumDetails.EmployeeScrumAnswers = mockEmployeeScrumAnswers;

        let fixture = TestBed.createComponent(ScrumProjectDetailComponent);
        let activatedRoute = fixture.debugElement.injector.get(ActivatedRoute);
        activatedRoute.params = Observable.of({ Id: stringConstant.userId });

        let scrumReportService = fixture.debugElement.injector.get(ScrumReportService);
        spyOn(scrumReportService, "getScrumDetails").and.returnValue(new BehaviorSubject(mockScrumDetails).asObservable());
        let scrumProjectDetailComponent = fixture.componentInstance;
        scrumProjectDetailComponent.getScrumDetailsYesterday(new Date().toDateString());
        expect(scrumProjectDetailComponent.employeeScrumAnswers.length).toBe(1);
    });

    it('Shows scrum answers of employees in a project on initialization', () => {
        let mockScrumDetails = new ScrumDetails();
        let mockEmployeeScrumAnswers = new Array<EmployeeScrumAnswers>();
        let mockEmployeeScrumAnswer = new EmployeeScrumAnswers();
        mockEmployeeScrumAnswer.Answer1 = stringConstant.Answer1;
        mockEmployeeScrumAnswer.Answer2 = stringConstant.Answer2;
        mockEmployeeScrumAnswer.Answer3 = stringConstant.Answer3;
        mockEmployeeScrumAnswer.EmployeeName = stringConstant.EmployeeName;
        mockEmployeeScrumAnswers.push(mockEmployeeScrumAnswer);
        mockScrumDetails.ScrumDate = stringConstant.ProjectCreationDate;
        mockScrumDetails.ProjectCreationDate = stringConstant.ProjectCreationDate;
        mockScrumDetails.EmployeeScrumAnswers = mockEmployeeScrumAnswers;
        let fixture = TestBed.createComponent(ScrumProjectDetailComponent);

        let activatedRoute = fixture.debugElement.injector.get(ActivatedRoute);
        activatedRoute.params = Observable.of({ Id: stringConstant.userId });

        let scrumReportService = fixture.debugElement.injector.get(ScrumReportService);
        spyOn(scrumReportService, "getScrumDetails").and.returnValue(new BehaviorSubject(mockScrumDetails).asObservable());
        let scrumProjectDetailComponent = fixture.componentInstance;
        scrumProjectDetailComponent.getScrumDetailsGeneral(new Date().toDateString());
        expect(scrumProjectDetailComponent.employeeScrumAnswers.length).toBe(1);
    });

    it('should be rediration to scrum details', fakeAsync(() => {
        let fixture = TestBed.createComponent(ScrumProjectDetailComponent);
        let scrumProjectDetailComponent = fixture.componentInstance;
        let router = fixture.debugElement.injector.get(Router);
        spyOn(router, "navigate");
        scrumProjectDetailComponent.goBack();
        tick();
        expect(router.navigate).toHaveBeenCalled();
    }));

});
