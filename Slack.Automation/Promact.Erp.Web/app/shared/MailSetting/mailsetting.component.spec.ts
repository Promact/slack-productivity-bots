declare let describe, it, beforeEach, expect, spyOn;
import { async, inject, TestBed, ComponentFixture, tick, fakeAsync } from '@angular/core/testing';
import { Provider } from "@angular/core";
import { Router, ActivatedRoute, RouterModule, Routes } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { RouterLinkStubDirective } from '../../shared/mock/mock.routerLink';
import { Project } from './project.model';
import { MailSettingService } from './mailsetting.service';
import { LoaderService } from '../../shared/loader.service';
import { Md2Toast } from 'md2';
import { MailSettingAC } from './mailsettingAC.model';
import { AppModule } from '../../app.module';
import { MockMailSettingService } from '../mock/mock.mailsetting.service';
import { MailSettingComponent } from './mailsetting.component';
import { MailSetting } from './mailsetting.model';
import { Md2SelectChange } from 'md2';
import { MockToast } from '../mock/mock.md2toast';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { StringConstant } from '../../shared/stringConstant';
import { MaterialAutoSelectChip } from '../angular-material-chip-autoselect.service';

let promise: TestBed;

let stringconstant = new StringConstant();

describe('Mail Setiings Component Test', () => {
    class MockRouter { }
    class MockLoaderService { }
    class MockMd2Select { }

    const routes: Routes = [];

    beforeEach(async(() => {
        this.promise = TestBed.configureTestingModule({
            declarations: [RouterLinkStubDirective], //Declaration of mock routerLink used on page.
            imports: [AppModule, RouterModule.forRoot(routes, { useHash: true }) //Set LocationStrategy for component. 
            ],
            providers: [
                { provide: LoaderService, useClass: MockLoaderService },
                { provide: Md2SelectChange, useClass: MockMd2Select },
                { provide: Md2Toast, useClass: MockToast },
                { provide: MaterialAutoSelectChip, useClass: MaterialAutoSelectChip }
            ]
        }).compileComponents();
    }));

    it('OnInit', fakeAsync(() => {
        let fixture = TestBed.createComponent(MailSettingComponent); //Create instance of component            
        let mailSettingComponent = fixture.componentInstance;
        let mailService = fixture.debugElement.injector.get(MailSettingService);
        let groupList = Array<string>();
        groupList = ["hello"];
        let listOfProject: Array<Project>;
        listOfProject = [new Project()];
        spyOn(mailService, stringconstant.getListOfGroups).and.returnValue(new BehaviorSubject(groupList).asObservable());
        spyOn(mailService, stringconstant.getAllProjects).and.returnValue(new BehaviorSubject(listOfProject).asObservable());
        mailSettingComponent.ngOnInit();
        tick();
        expect(mailSettingComponent.groupList.length).toBe(1);
        expect(mailSettingComponent.listOfProject.length).toBe(1);
        expect(mailSettingComponent.projectSelected).toBe(false);
    }));

    it('addMailSetting', fakeAsync(() => {
        let fixture = TestBed.createComponent(MailSettingComponent); //Create instance of component            
        let mailSettingComponent = fixture.componentInstance;
        let mailService = fixture.debugElement.injector.get(MailSettingService);
        let router = fixture.debugElement.injector.get(Router);
        let mailSetting = new MailSetting();
        mailSetting.Project = new Project();
        spyOn(mailService, stringconstant.addMailSetting).and.returnValue(new BehaviorSubject({}).asObservable());
        spyOn(router, stringconstant.navigate);
        mailSettingComponent.addMailSetting(mailSetting);
        tick();
        expect(router.navigate).toHaveBeenCalled();
    }));

    it('updateMailSetting', fakeAsync(() => {
        let fixture = TestBed.createComponent(MailSettingComponent); //Create instance of component            
        let mailSettingComponent = fixture.componentInstance;
        let mailService = fixture.debugElement.injector.get(MailSettingService);
        let router = fixture.debugElement.injector.get(Router);
        let mailSetting = new MailSetting();
        mailSetting.Project = new Project();
        spyOn(mailService, stringconstant.updateMailSetting).and.returnValue(new BehaviorSubject({}).asObservable());
        spyOn(router, stringconstant.navigate);
        mailSettingComponent.updateMailSetting(mailSetting);
        tick();
        expect(router.navigate).toHaveBeenCalled();
    }));

    it('getAllProject', fakeAsync(() => {
        let fixture = TestBed.createComponent(MailSettingComponent); //Create instance of component            
        let mailSettingComponent = fixture.componentInstance;
        let mailService = fixture.debugElement.injector.get(MailSettingService);
        let listOfProject: Array<Project>;
        listOfProject = [new Project()];
        spyOn(mailService, stringconstant.getAllProjects).and.returnValue(new BehaviorSubject(listOfProject).asObservable());
        mailSettingComponent.getAllProject();
        tick();
        expect(mailSettingComponent.listOfProject.length).toBe(1);
    }));

    it('getMailSettingDetailsByProjectId', fakeAsync(() => {
        let fixture = TestBed.createComponent(MailSettingComponent); //Create instance of component            
        let mailSettingComponent = fixture.componentInstance;
        let mailService = fixture.debugElement.injector.get(MailSettingService);
        let mailSetting: MailSetting = new MailSetting;
        mailSetting.Id = 1;
        spyOn(mailService, stringconstant.getProjectByIdAndModule).and.returnValue(new BehaviorSubject(mailSetting).asObservable());
        mailSettingComponent.getMailSettingDetailsByProjectId(1);
        tick();
        expect(mailSettingComponent.mailSetting.Id).toBe(1);
    }));

    it('getGroups', fakeAsync(() => {
        let fixture = TestBed.createComponent(MailSettingComponent); //Create instance of component            
        let mailSettingComponent = fixture.componentInstance;
        let mailService = fixture.debugElement.injector.get(MailSettingService);
        let groupList = Array<string>();
        groupList = stringconstant.testGroupList;
        spyOn(mailService, stringconstant.getListOfGroups).and.returnValue(new BehaviorSubject(groupList).asObservable());
        mailSettingComponent.getGroups();
        tick();
        expect(mailSettingComponent.groupList.length).toBe(1);
    }));

    it('selectGroup for To for null list', fakeAsync(() => {
        let fixture = TestBed.createComponent(MailSettingComponent); //Create instance of component            
        let mailSettingComponent = fixture.componentInstance;
        mailSettingComponent.selectGroup(stringconstant.testName, true);
        tick();
        expect(mailSettingComponent.mailSetting.To.length).toBe(1);
        expect(mailSettingComponent.toHasValue).toBe(true);
    }));

    it('selectGroup for To for existing list', fakeAsync(() => {
        let fixture = TestBed.createComponent(MailSettingComponent); //Create instance of component            
        let mailSettingComponent = fixture.componentInstance;
        mailSettingComponent.mailSetting.To = stringconstant.testGroupList;
        mailSettingComponent.selectGroup(stringconstant.testName, true);
        tick();
        expect(mailSettingComponent.toHasValue).toBe(true);
    }));

    it('selectGroup for CC for null list', fakeAsync(() => {
        let fixture = TestBed.createComponent(MailSettingComponent); //Create instance of component            
        let mailSettingComponent = fixture.componentInstance;
        mailSettingComponent.selectGroup(stringconstant.testName, false);
        tick();
        expect(mailSettingComponent.mailSetting.CC.length).toBe(1);
        expect(mailSettingComponent.toHasValue).toBe(false);
    }));

    it('selectGroup for CC for existing list', fakeAsync(() => {
        let fixture = TestBed.createComponent(MailSettingComponent); //Create instance of component            
        let mailSettingComponent = fixture.componentInstance;
        mailSettingComponent.mailSetting.CC = stringconstant.testGroupList;
        mailSettingComponent.selectGroup(stringconstant.testName, false);
        tick();
        expect(mailSettingComponent.toHasValue).toBe(false);
    }));

    it('removeGroup for To and list is empty', fakeAsync(() => {
        let fixture = TestBed.createComponent(MailSettingComponent); //Create instance of component            
        let mailSettingComponent = fixture.componentInstance;
        mailSettingComponent.mailSetting.To = stringconstant.testGroupList;
        mailSettingComponent.removeGroup(stringconstant.testName, true);
        tick();
        expect(mailSettingComponent.mailSetting.To.length).toBe(0);
        expect(mailSettingComponent.toHasValue).toBe(false);
    }));

    it('removeGroup for To but list not empty', fakeAsync(() => {
        let fixture = TestBed.createComponent(MailSettingComponent); //Create instance of component            
        let mailSettingComponent = fixture.componentInstance;
        mailSettingComponent.mailSetting.To = stringconstant.testGroupListMultiValue;
        mailSettingComponent.removeGroup(stringconstant.testName, true);
        tick();
        expect(mailSettingComponent.mailSetting.To.length).toBe(1);
        expect(mailSettingComponent.toHasValue).toBe(true);
    }));

    it('removeGroup for CC', fakeAsync(() => {
        let fixture = TestBed.createComponent(MailSettingComponent); //Create instance of component            
        let mailSettingComponent = fixture.componentInstance;
        mailSettingComponent.mailSetting.CC = stringconstant.testGroupList;
        mailSettingComponent.selectGroup(stringconstant.testName, false);
        tick();
        expect(mailSettingComponent.toHasValue).toBe(false);
    }));
});
