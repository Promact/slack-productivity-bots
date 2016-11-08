declare let describe, it, beforeEach, expect;
import { async, inject, TestBed, ComponentFixture } from '@angular/core/testing';
import { provide, Component, OnInit } from "@angular/core";
import { ActivatedRoute } from '@angular/router';
import { ScrumProjectDetailComponent } from './scrumProject-Details.component';
import { ScrumReportService } from '../scrumReport.service';
import { TestConnection } from "../../shared/mock/test.connection";
import { MockScrumReportService } from '../../shared/mock/mock.scrumReport.service';
import { Observable } from 'rxjs/Observable';
import { StringConstant } from '../../shared/stringConstant';

describe('ScrumReport Tests', () => {
    let scrumProjectDetailComponent: ScrumProjectDetailComponent;
    let router: ActivatedRoute;
    class MockActivatedRoute extends ActivatedRoute {
        constructor() {
            super();
            this.params = Observable.of({ id: "123" });
        }
    }

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [
                provide(ActivatedRoute, { useClass: MockActivatedRoute }),
                provide(TestConnection, { useClass: TestConnection }),
                provide(ScrumReportService, { useClass: MockScrumReportService }),
                provide(StringConstant, { useClass: StringConstant }),
            ]
        });
    });

    beforeEach(inject([ScrumReportService, ActivatedRoute, StringConstant], (scrumReportService: ScrumReportService, mockRouter: ActivatedRoute, stringConstant: StringConstant) => {
        scrumProjectDetailComponent = new ScrumProjectDetailComponent(scrumReportService, mockRouter, stringConstant);
    }));


    it('Shows scrum answers of employees in a project on initialization', () => {
        scrumProjectDetailComponent.ngOnInit();
        expect(scrumProjectDetailComponent.employeeScrumAnswers.length).toBe(1);
    });
})
