declare let describe, it, beforeEach, expect;
import { async, inject, TestBed, ComponentFixture } from '@angular/core/testing';
import { provide, Component, OnInit } from "@angular/core";
import { Router } from '@angular/router';
import { LeaveReportListComponent } from './leaveReport-List.component';
import { LeaveReportService } from '../leaveReport.service';
import { TestConnection } from "../../shared/mock/test.connection";
import { MockLeaveReportService } from '../../shared/mock/mock.leaveReport.service';
import { StringConstant } from '../../shared/stringConstant';

describe('LeaveReport Tests', () => {
    let leaveReportListComponent: LeaveReportListComponent;
    let router: Router;
    class MockRouter { }

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [
                provide(Router, { useClass: MockRouter }),
                provide(TestConnection, { useClass: TestConnection }),
                provide(LeaveReportService, { useClass: MockLeaveReportService }),
                provide(StringConstant, { useClass: StringConstant }),
            ]
        });
    });

    beforeEach(inject([LeaveReportService, Router, StringConstant], (leaveReportService: LeaveReportService, mockRouter: Router, stringConstant: StringConstant) => {
        router = mockRouter;
        leaveReportListComponent = new LeaveReportListComponent(leaveReportService, router, stringConstant);
    }));


    it('Shows list of leaveReports on initialization', () => {
        leaveReportListComponent.ngOnInit();
        expect(leaveReportListComponent.leaveReports.length).toBe(1);
    });

    //it('Downloads report of leave reports on export to pdf', () => {
    //    leaveReportComponent.exportDataToPdf();
    //    console.log(leaveReportComponent.leaveReports.push());
    //    expect(leaveReportComponent.exportDataToPdf).toBe();
    //});
})





