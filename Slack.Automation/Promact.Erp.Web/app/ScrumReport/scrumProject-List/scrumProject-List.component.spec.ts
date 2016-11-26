
declare var describe, it, beforeEach, expect;
import { async, inject, TestBed, ComponentFixture } from '@angular/core/testing';
import { Provider } from "@angular/core";
import { Router, ActivatedRoute, RouterModule, Routes } from '@angular/router';
import { RouterLinkStubDirective } from '../../shared/mock/mock.routerLink';
import { ScrumModule } from '../scrumReport.module';
import { ScrumProjectListComponent } from './scrumProject-List.component';
import { ScrumReportService } from '../scrumReport.service';
import { MockScrumReportService } from '../../shared/mock/mock.scrumReport.service';
import { StringConstant } from '../../shared/stringConstant';

let promise: TestBed;   

describe('ScrumReport Tests', () => {

    class MockRouter { }
    const routes: Routes = [];

    beforeEach(async(() => {
        this.promise = TestBed.configureTestingModule({
            declarations: [RouterLinkStubDirective],
            imports: [ScrumModule, RouterModule.forRoot(routes, { useHash: true })],
            providers: [
                { provide: Router, useClass: MockRouter },
                { provide: ScrumReportService, useClass: MockScrumReportService },
                { provide: StringConstant, useClass: StringConstant },
            ]
        }).compileComponents();
    }));

    it('Shows list of projects on initialization', () => done => {
        this.promise.then(() => {
            let fixture = TestBed.createComponent(ScrumProjectListComponent);
            let scrumProjectListComponent = fixture.componentInstance;
            let result = scrumProjectListComponent.ngOnInit();
            expect(scrumProjectListComponent.scrumProjects.length).toBe(1);
            done();
        });
    });
});
