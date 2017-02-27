declare let describe, it, beforeEach, expect;
import { async, inject, TestBed, ComponentFixture } from '@angular/core/testing';
import { Provider } from "@angular/core";
import { Router, ActivatedRoute, RouterModule, Routes } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { RouterLinkStubDirective } from '../../shared/mock/mock.routerLink';
import { LeaveModule } from '../../leaveReport/leaveReport.module';
import { LeaveReportService } from '../leaveReport.service';
import { MockLeaveReportService } from '../../shared/mock/mock.leaveReport.service';
import { StringConstant } from '../../shared/stringConstant';
import { LeaveReportListComponent } from './leaveReport-List.component';
import { LoaderService } from '../../shared/loader.service';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { LeaveReport } from '../../leaveReport/leaveReport-List/leaveReport-List.model';
import { LeaveReportDetail } from '../../leaveReport/leaveReport-Details/leaveReport-Details.model';

let promise: TestBed;


describe('LeaveReport List Tests', () => {
    class MockRouter { }
    class MockLoaderService { }

    const routes: Routes = [];

    beforeEach(async(() => {
        this.promise = TestBed.configureTestingModule({
            declarations: [RouterLinkStubDirective], //Declaration of mock routerLink used on page.
            imports: [LeaveModule, RouterModule.forRoot(routes, { useHash: true }) //Set LocationStrategy for component. 
            ],
            providers: [
                { provide: Router, useClass: MockRouter },
                { provide: LeaveReportService, useClass: MockLeaveReportService },
                { provide: StringConstant, useClass: StringConstant },
                { provide: LoaderService, useClass: MockLoaderService }
            ]
        }).compileComponents();
    }));


    it('Shows list of leaveReports on initialization', () => {
        let mockLeaveReports = new Array<MockLeaveReport>();
        let mockLeaveReport = new MockLeaveReport();
        mockLeaveReport.EmployeeId = "abc";
        mockLeaveReport.EmployeeUserName = "abc@abc.com";
        mockLeaveReport.EmployeeName = "abc";
        mockLeaveReport.TotalSickLeave = 7;
        mockLeaveReport.TotalCasualLeave = 14;
        mockLeaveReports.push(mockLeaveReport);

        let fixture = TestBed.createComponent(LeaveReportListComponent); //Create instance of component            
        let leaveReportListComponent = fixture.componentInstance;
        let leaveReportService = fixture.debugElement.injector.get(LeaveReportService);
        spyOn(leaveReportService, "getLeaveReports").and.returnValue(new BehaviorSubject(mockLeaveReports).asObservable());
        let result = leaveReportListComponent.getLeaveReports();
        expect(leaveReportListComponent.leaveReports.length).toBe(1);
    });


    it('Shows list of leaveReports on initialization but no reports', () => {
        let mockLeaveReports = new Array<MockLeaveReport>();
        let fixture = TestBed.createComponent(LeaveReportListComponent); //Create instance of component            
        let leaveReportListComponent = fixture.componentInstance;
        let leaveReportService = fixture.debugElement.injector.get(LeaveReportService);
        spyOn(leaveReportService, "getLeaveReports").and.returnValue(new BehaviorSubject(mockLeaveReports).asObservable());
        let result = leaveReportListComponent.getLeaveReports();
        expect(leaveReportListComponent.leaveReports.length).toBe(0);
    });

    it('Downloads report of leave reports on export to pdf', () => {
        let fixture = TestBed.createComponent(LeaveReportListComponent); //Create instance of component            
        let leaveReportListComponent = fixture.componentInstance;
        let leaveReportService = fixture.debugElement.injector.get(LeaveReportService);
        spyOn(leaveReportListComponent, "exportDataToPdf");
        let result = leaveReportListComponent.exportDataToPdf();
        expect(leaveReportListComponent.exportDataToPdf).toHaveBeenCalled();
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









