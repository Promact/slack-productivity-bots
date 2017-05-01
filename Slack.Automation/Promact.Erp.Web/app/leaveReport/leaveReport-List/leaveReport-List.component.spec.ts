declare let describe, it, beforeEach, expect, spyOn;
import { async, inject, TestBed, ComponentFixture } from '@angular/core/testing';
import { Provider } from "@angular/core";
import { Router, ActivatedRoute, RouterModule, Routes } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { RouterLinkStubDirective } from '../../shared/mock/mock.routerLink';
import { AppModule } from '../../../app/app.module';
import { LeaveReportService } from '../leaveReport.service';
import { MockLeaveReportService } from '../../shared/mock/mock.leaveReport.service';
import { StringConstant } from '../../shared/stringConstant';
import { LeaveReportListComponent } from './leaveReport-List.component';
import { LoaderService } from '../../shared/loader.service';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { LeaveReport } from '../../leaveReport/leaveReport-List/leaveReport-List.model';
import { LeaveReportDetail } from '../../leaveReport/leaveReport-Details/leaveReport-Details.model';
import { MockRouter } from '../../shared/mock/mock.router';
import { ActivatedRouteStub } from '../../shared/mock/mock.activatedroute';
let promise: TestBed;
let stringConstant = new StringConstant();
import { JsonToPdfService } from '../../shared/jsontopdf.service';

describe('LeaveReport List Tests', () => {
    class MockLoaderService { }

    const routes: Routes = [];

    beforeEach(async(() => {
        this.promise = TestBed.configureTestingModule({
            declarations: [RouterLinkStubDirective], //Declaration of mock routerLink used on page.
            imports: [AppModule, RouterModule.forRoot(routes, { useHash: true }) //Set LocationStrategy for component. 
            ],
            providers: [
                { provide: Router, useClass: MockRouter },
                { provide: LeaveReportService, useClass: MockLeaveReportService },
                { provide: StringConstant, useClass: StringConstant },
                { provide: LoaderService, useClass: LoaderService }
            ]
        }).compileComponents();
    }));


    it('Shows list of leaveReports', () => {
        let fixture = TestBed.createComponent(LeaveReportListComponent); //Create instance of component  
        let leaveReportListComponent = fixture.componentInstance;
        let result = leaveReportListComponent.ngOnInit();
        expect(leaveReportListComponent.leaveReports.length).toBe(1);
    });


    it('Shows list of leaveReports on initialization', () => {
        let mockLeaveReports = new Array<MockLeaveReport>();
        let mockLeaveReport = new MockLeaveReport();
        mockLeaveReport.EmployeeId = stringConstant.userId;
        mockLeaveReport.EmployeeUserName = stringConstant.userName;
        mockLeaveReport.EmployeeName = stringConstant.userEmail;
        mockLeaveReport.TotalSickLeave = parseInt(stringConstant.sickLeave);
        mockLeaveReport.TotalCasualLeave = parseInt(stringConstant.casualLeave);
        mockLeaveReports.push(mockLeaveReport);

        let fixture = TestBed.createComponent(LeaveReportListComponent); //Create instance of component            
        let leaveReportListComponent = fixture.componentInstance;
        let leaveReportService = fixture.debugElement.injector.get(LeaveReportService);
        spyOn(leaveReportService, stringConstant.getLeaveReports).and.returnValue(new BehaviorSubject(mockLeaveReports).asObservable());
        let result = leaveReportListComponent.getLeaveReports();
        expect(leaveReportListComponent.leaveReports.length).toBe(1);
    });


    it('Shows list of leaveReports on initialization but no reports', () => {
        let mockLeaveReports = new Array<MockLeaveReport>();
        let fixture = TestBed.createComponent(LeaveReportListComponent); //Create instance of component            
        let leaveReportListComponent = fixture.componentInstance;
        let leaveReportService = fixture.debugElement.injector.get(LeaveReportService);
        spyOn(leaveReportService, stringConstant.getLeaveReports).and.returnValue(new BehaviorSubject(mockLeaveReports).asObservable());
        let result = leaveReportListComponent.getLeaveReports();
        expect(leaveReportListComponent.leaveReports.length).toBe(0);
    });

   
    it('Downloads report of leave reports on export to pdf', () => {
        let mockLeaveReports = new Array<MockLeaveReport>();
        let mockLeaveReport = new MockLeaveReport();
        mockLeaveReport.EmployeeId = stringConstant.userId;
        mockLeaveReport.EmployeeUserName = stringConstant.userEmail;
        mockLeaveReport.EmployeeName = stringConstant.userEmail;
        mockLeaveReport.TotalSickLeave = parseInt(stringConstant.sickLeave);
        mockLeaveReport.TotalCasualLeave = parseInt(stringConstant.casualLeave);
        mockLeaveReports.push(mockLeaveReport);
        let fixture = TestBed.createComponent(LeaveReportListComponent); //Create instance of component   
        let jsPDFMock = fixture.debugElement.injector.get(JsonToPdfService);
        spyOn(jsPDFMock, "exportJsonToPdf").and.callFake(function fake() { });
        let leaveReportListComponent = fixture.componentInstance;
        leaveReportListComponent.leaveReports = mockLeaveReports;
        leaveReportListComponent.exportDataToPdf();
        console.log(leaveReportListComponent.leaveReports.push());
        expect(leaveReportListComponent.leaveReports.length).toBe(1);
    });

});


class MockLeaveReport extends LeaveReport {
    constructor() {
        super();
    }
}


class MockLeaveReportDetails extends LeaveReportDetail {
    constructor() {
        super();
    }
}