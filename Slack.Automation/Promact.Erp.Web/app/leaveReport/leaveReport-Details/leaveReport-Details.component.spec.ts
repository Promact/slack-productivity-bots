declare let describe, it, beforeEach, expect;
import { async, inject, TestBed, ComponentFixture } from '@angular/core/testing';
import { provide, Component, OnInit } from "@angular/core";
import { ActivatedRoute } from '@angular/router';
import { LeaveReportDetailsComponent } from './leaveReport-Details.component';
import { LeaveReportService } from '../leaveReport.service';
import { TestConnection } from "../../mock/test.connection";
import { MockLeaveReportService } from '../../mock/mock.leaveReport.service';
import { Observable } from 'rxjs/Observable';

describe('LeaveReport Tests', () => {
    let leaveReportDetailsComponent: LeaveReportDetailsComponent;
    let router: ActivatedRoute;
    class MockActivatedRoute extends ActivatedRoute {
        constructor() {
            super();
            this.params = Observable.of({ id: "abc" });
        }
    }

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [
                provide(ActivatedRoute, { useClass: MockActivatedRoute }),
                provide(TestConnection, { useClass: TestConnection }),
                provide(LeaveReportService, { useClass: MockLeaveReportService }),
            ]
        });
    });

    beforeEach(inject([LeaveReportService, ActivatedRoute], (leaveReportService: LeaveReportService, mockRouter: ActivatedRoute) => {
        router = mockRouter;
        leaveReportDetailsComponent = new LeaveReportDetailsComponent(leaveReportService, router);
    }));


    it('Shows details of leave report for an employee on initialization', () => {
        leaveReportDetailsComponent.ngOnInit();
        expect(leaveReportDetailsComponent.leaveReportDetail.length).toBe(1);
    });
})
