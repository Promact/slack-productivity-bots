declare let describe, it, beforeEach, expect;
import { async, TestBed } from '@angular/core/testing';
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

let promise: TestBed;

describe('AppComponent Test', () => {
    class MockRouter { }
    class MockLoaderService { }
    const routes: Routes = [];

    beforeEach(async(() => {
        this.promise = TestBed.configureTestingModule({
            declarations: [RouterLinkStubDirective], //Declaration of mock routerLink used on page.
            imports: [AppModule, RouterModule.forRoot(routes, { useHash: true }) //Set LocationStrategy for component. 
            ],
            providers: [
                { provide: Router, useClass: MockRouter },
                { provide: AppComponentService, useClass: MockAppComponentService },
                { provide: LoaderService, useClass: MockLoaderService },
            ]
        }).compileComponents();
    }));

    it('User is admin', () => done => {
        this.promise.then(() => {
            let fixture = TestBed.createComponent(AppComponent); //Create instance of component            
            let appComponent = fixture.componentInstance;
            let result = appComponent.ngOnInit();
            expect(appComponent.userIsAdmin).toBe(true);
            done();
        });
    });
})