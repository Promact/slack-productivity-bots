declare var describe, it, beforeEach, expect;
import { async, inject, TestBed, ComponentFixture, tick, fakeAsync } from '@angular/core/testing';
import { Provider } from "@angular/core";
import { Router, ActivatedRoute, RouterModule, Routes } from '@angular/router';
import { RouterLinkStubDirective } from '../../shared/mock/mock.routerLink';
import { ScrumModule } from '../scrumReport.module';
import { ScrumProjectListComponent } from './scrumProject-List.component';
import { ScrumReportService } from '../scrumReport.service';
import { MockScrumReportService } from '../../shared/mock/mock.scrumReport.service';
import { StringConstant } from '../../shared/stringConstant';
import { LoaderService } from '../../shared/loader.service';
import { AppModule } from '../../app.module';
import { TestConnection } from '../../shared/mock/test.connection';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { ScrumProject } from './scrumProject-List.model';
import { MailSettingModule } from '../../shared/MailSetting/mailsetting.module';

let promise: TestBed;

let stringConstant = new StringConstant();
describe('ScrumReport Tests', () => {
    const routes: Routes = [];

    beforeEach(async(() => {
        this.promise = TestBed.configureTestingModule({
            declarations: [RouterLinkStubDirective],
            imports: [ScrumModule, MailSettingModule, RouterModule.forRoot(routes, { useHash: true })],
            providers: [
                { provide: ScrumReportService, useClass: MockScrumReportService },    
                { provide: StringConstant, useClass: StringConstant },
                { provide: LoaderService, useClass: LoaderService },
            ]
        }).compileComponents();
    }));

    it('Shows list of projects on initialization', fakeAsync(() => {
        let fixture = TestBed.createComponent(ScrumProjectListComponent);
        let scrumProjectListComponent = fixture.componentInstance;
        let scrumService = fixture.debugElement.injector.get(ScrumReportService);
        let router = fixture.debugElement.injector.get(Router);
        let mockScrumProjects = new Array<ScrumProject>();
        let scrumProject: ScrumProject;
        scrumProject = new ScrumProject();
        scrumProject.Name = stringConstant.scrumName;
        mockScrumProjects.push(scrumProject);
        spyOn(scrumService, "getScrumProjects").and.returnValue(new BehaviorSubject(mockScrumProjects).asObservable());
        let result = scrumProjectListComponent.ngOnInit();
        tick(); 
        expect(scrumProjectListComponent.scrumProjects.length).toBe(1);
    }));

    it('Shows list of projects on initialization', fakeAsync(() => {
        let fixture = TestBed.createComponent(ScrumProjectListComponent);
        let scrumProjectListComponent = fixture.componentInstance;
        let scrumService = fixture.debugElement.injector.get(ScrumReportService);
        let router = fixture.debugElement.injector.get(Router);
        let mockScrumProjects = new Array<ScrumProject>();
        let scrumProject: ScrumProject;
        scrumProject = new ScrumProject();
        scrumProject.Name = stringConstant.scrumName;
        mockScrumProjects.push(scrumProject);
        spyOn(scrumService, "getScrumProjects").and.returnValue(new BehaviorSubject(mockScrumProjects).asObservable());
        let result = scrumProjectListComponent.getScrumProjects();
        tick();
        expect(scrumProjectListComponent.scrumProjects.length).toBe(1);
    }));


    it('Shows list of projects on initialization', fakeAsync(() => {
        let fixture = TestBed.createComponent(ScrumProjectListComponent);
        let scrumProjectListComponent = fixture.componentInstance;
        let scrumService = fixture.debugElement.injector.get(ScrumReportService);
        let router = fixture.debugElement.injector.get(Router);
        let scrumProjects: Array<ScrumProject>;
        scrumProjects = new Array<ScrumProject>();
        spyOn(scrumService, "getScrumProjects").and.returnValue(new BehaviorSubject(scrumProjects).asObservable());
        let result = scrumProjectListComponent.getScrumProjects();
        tick();
        expect(scrumProjectListComponent.noProject).toBe(true);
    }));
});
