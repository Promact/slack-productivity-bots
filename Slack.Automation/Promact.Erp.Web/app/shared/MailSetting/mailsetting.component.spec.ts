declare let describe, it, beforeEach, expect;
import { async, inject, TestBed, ComponentFixture } from '@angular/core/testing';
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
let promise: TestBed;

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
                { provide: Router, useClass: MockRouter },
                { provide: MailSettingService, useClass: MockMailSettingService },
                { provide: LoaderService, useClass: MockLoaderService },
                { provide: Md2SelectChange, useClass: MockMd2Select },
            ]
        }).compileComponents();
    }));

    it('OnInit', () => done => {
        this.promise.then(() => {
            let fixture = TestBed.createComponent(MailSettingComponent); //Create instance of component            
            let mailSettingComponent = fixture.componentInstance;
            let result = mailSettingComponent.ngOnInit();
            expect(mailSettingComponent.groupList.length).toBe(1);
            done();
        });
    });

    it('addMailSetting', () => done => {
        this.promise.then(() => {
            let fixture = TestBed.createComponent(MailSettingComponent); //Create instance of component            
            let mailSettingComponent = fixture.componentInstance;
            let mailSetting: MailSetting = new MailSetting;
            mailSetting.Id = 1;
            let result = mailSettingComponent.addMailSetting(mailSetting);
            expect(mailSettingComponent.currentModule).toBe("");
            done();
        });
    });

    it('updateMailSetting', () => done => {
        this.promise.then(() => {
            let fixture = TestBed.createComponent(MailSettingComponent); //Create instance of component            
            let mailSettingComponent = fixture.componentInstance;
            let mailSetting: MailSetting = new MailSetting;
            mailSetting.Id = 1;
            let result = mailSettingComponent.updateMailSetting(mailSetting);
            expect(mailSettingComponent.currentModule).tobe("");
            done();
        });
    });

    it('getAllProject', () => done => {
        this.promise.then(() => {
            let fixture = TestBed.createComponent(MailSettingComponent); //Create instance of component            
            let mailSettingComponent = fixture.componentInstance;
            let mailSetting: MailSetting = new MailSetting;
            mailSetting.Id = 1;
            let result = mailSettingComponent.getAllProject();
            expect(mailSettingComponent.listOfProject).tobe(1);
            done();
        });
    });

    it('getMailSettingDetailsByProjectId', () => done => {
        this.promise.then(() => {
            let fixture = TestBed.createComponent(MailSettingComponent); //Create instance of component            
            let mailSettingComponent = fixture.componentInstance;
            let mailSetting: MailSetting = new MailSetting;
            mailSetting.Id = 1;
            let result = mailSettingComponent.getMailSettingDetailsByProjectId(1);
            expect(mailSettingComponent.mailSetting.Id).tobe(1);
            done();
        });
    });

    it('getMailSettingDetailsByProjectId', () => done => {
        this.promise.then(() => {
            let fixture = TestBed.createComponent(MailSettingComponent); //Create instance of component            
            let mailSettingComponent = fixture.componentInstance;
            let mailSetting: MailSetting = new MailSetting;
            mailSetting.Id = 1;
            let result = mailSettingComponent.getMailSettingDetailsByProjectId(1);
            expect(mailSettingComponent.mailSetting.Id).tobe(1);
            expect(mailSettingComponent.IsToUpdate).tobe(false);
            done();
        });
    });

    it('getGroups', () => done => {
        this.promise.then(() => {
            let fixture = TestBed.createComponent(MailSettingComponent); //Create instance of component            
            let mailSettingComponent = fixture.componentInstance;
            let mailSetting: MailSetting = new MailSetting;
            mailSetting.Id = 1;
            let result = mailSettingComponent.getGroups();
            expect(mailSettingComponent.groupList.length).tobe(1);
            done();
        });
    });
})