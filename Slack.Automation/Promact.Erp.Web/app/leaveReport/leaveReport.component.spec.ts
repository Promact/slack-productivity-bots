/// <reference path="leavereport.service.ts" />
/// <reference path="leavereport.component.ts" />
declare let describe, it, beforeEach, expect;
import { async, inject, TestBed, ComponentFixture } from '@angular/core/testing';
import { Provider } from "@angular/core";
import { Router, ActivatedRoute, RouterModule, Routes } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { RouterLinkStubDirective } from '../shared/mock/mock.routerLink';
import { AppModule } from '../../app/app.module';
import { LeaveReportService } from './leaveReport.service';
import { MockLeaveReportService } from '../shared/mock/mock.leaveReport.service';
import { StringConstant } from '../shared/stringConstant';
import { LeaveReportComponent } from '../leaveReport/leaveReport.component';
import { LoaderService } from '../shared/loader.service';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { LeaveReport } from '../leaveReport/leaveReport-List/leaveReport-List.model';
import { LeaveReportDetail } from '../leaveReport/leaveReport-Details/leaveReport-Details.model';
import { MockRouter } from '../shared/mock/mock.router';
import { ActivatedRouteStub } from '../shared/mock/mock.activatedroute';
let promise: TestBed;


describe('LeaveReport Component Tests', () => {
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

    it("The 'toBeDefined' matcher compares against `defined`", () => {
        let fixture = TestBed.createComponent(LeaveReportComponent); //Create instance of component  
        let leaveReportComponent = fixture.componentInstance;
        expect(leaveReportComponent).toBeDefined();
    });


});

