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
import { LeaveReportDetailsComponent } from './leaveReport-Details.component';

let promise: TestBed;

describe('LeaveReport Detials Tests', () => {
    const routes: Routes = [];
    class MockActivatedRoute extends ActivatedRoute {
        constructor() {
            super();
            this.params = Observable.of({ id: "1" });
        }
    };

    beforeEach(async(() => {
        this.promise = TestBed.configureTestingModule({
            declarations: [RouterLinkStubDirective], //Declaration of mock routerLink used on page.
            imports: [LeaveModule, RouterModule.forRoot(routes, { useHash: true }) //Set LocationStrategy for component. 
            ],
            providers: [
                { provide: ActivatedRoute, useClass: MockActivatedRoute },
                { provide: LeaveReportService, useClass: MockLeaveReportService },
                { provide: StringConstant, useClass: StringConstant },
            ]
        }).compileComponents();
    }));

    it('Shows details of leave report for an employee on initialization', () => done => {
        this.promise.then(() => {
            let fixture = TestBed.createComponent(LeaveReportDetailsComponent); //Create instance of component            
            let leaveReportDetailsComponent = fixture.componentInstance;
            let result = leaveReportDetailsComponent.ngOnInit();
            expect(leaveReportDetailsComponent.leaveReportDetail.length).toBe(1);
            done();
        });
    });

});






