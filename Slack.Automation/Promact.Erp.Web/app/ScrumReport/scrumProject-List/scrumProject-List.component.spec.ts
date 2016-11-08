
declare let describe, it, beforeEach, expect;
import { async, inject, TestBed, ComponentFixture } from '@angular/core/testing';
import { provide, Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { ScrumProjectListComponent } from './scrumProject-List.component';
import { ScrumReportService } from '../scrumReport.service';
import { TestConnection } from '../../shared/mock/test.connection';
import { MockScrumReportService } from '../../shared/mock/mock.scrumReport.service';
import { StringConstant } from '../../shared/stringConstant';

describe('ScrumReport Tests', () => {
    let scrumProjectListComponent: ScrumProjectListComponent;
    let router: Router;
    class MockRouter { }

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [
                provide(Router, { useClass: MockRouter }),
                provide(TestConnection, { useClass: TestConnection }),
                provide(ScrumReportService, { useClass: MockScrumReportService }),
                provide(StringConstant, { useClass: StringConstant }),
            ]
        });
    });


    beforeEach(inject([ScrumReportService, Router], (scrumReportService: ScrumReportService, mockRouter: Router) => {
        scrumProjectListComponent = new ScrumProjectListComponent(scrumReportService, mockRouter);
    }));

    it('Shows list of projects on initialization', () => {
        scrumProjectListComponent.ngOnInit();
        expect(scrumProjectListComponent.scrumProjects.length).toBe(1);
    });
})
