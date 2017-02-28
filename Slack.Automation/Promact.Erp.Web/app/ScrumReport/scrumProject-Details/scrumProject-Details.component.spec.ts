//declare var describe, it, beforeEach, expect;
//import { async, inject, TestBed, ComponentFixture, tick, fakeAsync } from '@angular/core/testing';
//import { Provider } from "@angular/core";
//import { Router, ActivatedRoute, RouterModule, Routes } from '@angular/router';
//import { RouterLinkStubDirective } from '../../shared/mock/mock.routerLink';
//import { ScrumModule } from '../scrumReport.module';
//import { ScrumProjectDetailComponent } from './scrumProject-Details.component';
//import { ScrumReportService } from '../scrumReport.service';
//import { TestConnection } from "../../shared/mock/test.connection";
//import { MockScrumReportService } from '../../shared/mock/mock.scrumReport.service';
//import { Observable } from 'rxjs/Observable';
//import { StringConstant } from '../../shared/stringConstant';
//import { LoaderService } from '../../shared/loader.service';
//import { Md2SelectChange } from 'md2';
//import { AppModule } from '../../app.module';
//import { BehaviorSubject } from 'rxjs/BehaviorSubject';
//import { ScrumDetails } from '../../ScrumReport/scrumProject-Details/scrumProject-Details.model';
//import { EmployeeScrumAnswers } from '../../ScrumReport/scrumProject-Details/scrumProject-EmployeeScrumDetails.model';
//let promise: TestBed;


//describe('ScrumReport Tests', () => {
//    const routes: Routes = [];
//    class MockLoaderService { }
//    class MockMd2Select { }
//    class MockActivatedRoute extends ActivatedRoute {
//        constructor() {
//            super();
//            this.params = Observable.of({ id: "123" });
//        }
//    }

//    beforeEach(async(() => {
//        this.promise = TestBed.configureTestingModule({
//            declarations: [RouterLinkStubDirective],
//            imports: [AppModule, RouterModule.forRoot(routes, { useHash: true })],
//            providers: [
//                { provide: ScrumReportService, useClass: MockScrumReportService },
//                { provide: StringConstant, useClass: StringConstant },
//                { provide: LoaderService, useClass: MockLoaderService },
//                { provide: Md2SelectChange, useClass: MockMd2Select }
//            ]
//        }).compileComponents();
//    }));
    
//    it('Shows scrum answers of employees in a project on initialization', fakeAsync(() => {
//            let fixture = TestBed.createComponent(ScrumProjectDetailComponent);
//            let scrumProjectDetailsComponent = fixture.componentInstance;
//            let scrumService = fixture.debugElement.injector.get(ScrumReportService);
//            let mockScrumDetails = new ScrumDetails();
//            let mockEmployeeScrumAnswers = new Array<EmployeeScrumAnswers>();
//            let mockEmployeeScrumAnswer = new EmployeeScrumAnswers();
//            mockEmployeeScrumAnswer.Answer1 = "abc";
//            mockEmployeeScrumAnswer.Answer2 = "abc2";
//            mockEmployeeScrumAnswer.Answer3 = "no";
//            mockEmployeeScrumAnswer.EmployeeName = "xyz";
//            mockEmployeeScrumAnswers.push(mockEmployeeScrumAnswer);
//            let date = new Date().toString();
//            let id = 123;
//            if (id === 123) {
//                mockScrumDetails.ScrumDate = date;
//                mockScrumDetails.ProjectCreationDate = "1/1/16";
//                mockScrumDetails.EmployeeScrumAnswers = mockEmployeeScrumAnswers;
//            }
//            let connection = this.getMockResponse(this.stringConstant.scrum + this.stringConstant.slash + id + this.stringConstant.detail, mockScrumDetails);
//            spyOn(scrumService, "getScrumDetails").and.returnValue(new BehaviorSubject(connection).asObservable());
//            let result = scrumProjectDetailsComponent.ngOnInit();
//            tick();
//            expect(scrumProjectDetailsComponent.employeeScrumAnswers.length).toBe(1);
//        }));
//});
