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

    it('Shows list of leaveReports on initialization', () => done => {
        this.promise.then(() => {
            let fixture = TestBed.createComponent(LeaveReportListComponent); //Create instance of component            
            let leaveReportListComponent = fixture.componentInstance;
            let result = leaveReportListComponent.ngOnInit();
            expect(leaveReportListComponent.leaveReports.length).toBe(1);
            done();
        });
    });


    it('Downloads report of leave reports on export to pdf', () => done => {
        this.promise.then(() => {
            let fixture = TestBed.createComponent(LeaveReportListComponent); //Create instance of component            
            let leaveReportListComponent = fixture.componentInstance;
            leaveReportListComponent.exportDataToPdf();
            console.log(leaveReportListComponent.leaveReports.push());
            expect(leaveReportListComponent.exportDataToPdf).toBe();
        });
    });

});












