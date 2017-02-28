declare let describe, it, beforeEach, expect;
import { async, TestBed, tick, fakeAsync } from '@angular/core/testing';
import { Provider } from "@angular/core";
import { Router, RouterModule, Routes } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { RouterLinkStubDirective } from './shared/mock/mock.routerLink';
import { LoaderService } from './shared/loader.service';
import { AppComponentService } from './appcomponent.service';
import { AppModule } from './app.module';
import { MockAppComponentService } from './shared/mock/mock.appcomponent.service';
import { AppComponent } from './app.component';
import { Md2SelectChange } from 'md2';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { EmailHashCode } from './shared/emailHashCode';
import { Project } from './shared/MailSetting/project.model';
let promise: TestBed;

describe('AppComponent Test', () => {
    class MockRouter { }
    class MockLoaderService { }
    class MockEmailHash { }
    const routes: Routes = [];

    beforeEach(async(() => {
        this.promise = TestBed.configureTestingModule({
            declarations: [RouterLinkStubDirective], //Declaration of mock routerLink used on page.
            imports: [AppModule, RouterModule.forRoot(routes, { useHash: true }) //Set LocationStrategy for component. 
            ],
            providers: [
                { provide: EmailHashCode, useClass: MockEmailHash },
                { provide: LoaderService, useClass: MockLoaderService },
            ]
        }).compileComponents();
    }));

    it('User is admin', () => {
            let fixture = TestBed.createComponent(AppComponent); //Create instance of component            
            let appComponent = fixture.componentInstance;
            let appService = fixture.debugElement.injector.get(AppComponentService);
            let result = "true";
            spyOn(appService, "getUserIsAdminOrNot").and.returnValue(new BehaviorSubject(result).asObservable());
            appComponent.ngOnInit();
            expect(appComponent.userIsAdmin).toBe(true);
    });
    it('User is not admin', () => {
        let fixture = TestBed.createComponent(AppComponent); //Create instance of component            
        let appComponent = fixture.componentInstance;
        let appService = fixture.debugElement.injector.get(AppComponentService);
        let result = "false";
        spyOn(appService, "getUserIsAdminOrNot").and.returnValue(new BehaviorSubject(result).asObservable());
        appComponent.ngOnInit();
        expect(appComponent.userIsAdmin).toBe(false);
    });
})