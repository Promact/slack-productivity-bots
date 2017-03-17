declare var describe, it, beforeEach, expect;
import { async, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { RouterModule, Routes } from '@angular/router';
import { RouterLinkStubDirective } from '../shared/mock/mock.routerLink';
import { ConfigurationModule } from './configuration.module';
import { ConfigurationComponent } from './configuration.component';
import { StringConstant } from '../shared/stringConstant';
import { Md2SelectChange, Md2Toast, Md2Dialog } from 'md2';
import { LoaderService } from '../shared/loader.service';
import { ConfigurationService } from './configuration.service';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { Configuration, ConfigurationStatusAC } from './configuration.model';
import { Router } from '@angular/router';
import { MockRouter } from '../shared/mock/mock.router';
import { SharedService } from '../shared/shared.service';
import { Http, Headers } from "@angular/http";
import { MockDialog } from '../shared/mock/mock.md2Dialog';

let promise: TestBed;
let stringConstant = new StringConstant;

describe('Configuration Component Test', () => {
    class MockRouter { }
    class MockLoaderService { }
    class MockMd2Select { }
    class MockToast { }
    class HttpMock { }
    const routes: Routes = [];

    beforeEach(async(() => {
        this.promise = TestBed.configureTestingModule({
            declarations: [RouterLinkStubDirective], //Declaration of mock routerLink used on page.
            imports: [ConfigurationModule, RouterModule.forRoot(routes, { useHash: true }) //Set LocationStrategy for component. 
            ],
            providers: [
                { provide: LoaderService, useClass: MockLoaderService },
                { provide: Md2SelectChange, useClass: MockMd2Select },
                { provide: Md2Toast, useClass: MockToast },
                { provide: SharedService, useClass: SharedService },
                { provide: Http, useClass: HttpMock },
                { provide: StringConstant, useClass: StringConstant },
                { provide: Md2Dialog, useClass: MockDialog }
            ]
        }).compileComponents();
    }));

    it('OnInit', fakeAsync(() => {
        let fixture = TestBed.createComponent(ConfigurationComponent); //Create instance of component            
        let configurationComponent = fixture.componentInstance;
        let configurationService = fixture.debugElement.injector.get(ConfigurationService);
        let listOfConfiguration: Array<Configuration>;
        listOfConfiguration = [new Configuration()];
        let router = fixture.debugElement.injector.get(Router);
        spyOn(router, stringConstant.navigate);
        spyOn(configurationService, stringConstant.getListOfConfiguration).and.returnValue(new BehaviorSubject(listOfConfiguration).asObservable());
        configurationComponent.ngOnInit();
        tick();
        expect(configurationComponent.configurationList.length).toBe(1);
    }));

    it('openModel status true', fakeAsync(() => {
        let fixture = TestBed.createComponent(ConfigurationComponent); //Create instance of component            
        let configurationComponent = fixture.componentInstance;
        let configurationService = fixture.debugElement.injector.get(ConfigurationService);
        let configuration = new Configuration();
        configuration.Module = stringConstant.taskModule;
        configuration.Status = true;
        configuration.Id = 1;
        let router = fixture.debugElement.injector.get(Router);
        spyOn(router, stringConstant.navigate);
        spyOn(configurationService, stringConstant.updateConfiguration).and.returnValue(new BehaviorSubject({}).asObservable());
        let popup = fixture.debugElement.injector.get(Md2Dialog);
        configurationComponent.openModel(configuration, popup);
        tick();
        expect(configurationComponent.configurationId).toBe(1);
    }));

    it('openModel status false', fakeAsync(() => {
        let fixture = TestBed.createComponent(ConfigurationComponent); //Create instance of component            
        let configurationComponent = fixture.componentInstance;
        let configurationService = fixture.debugElement.injector.get(ConfigurationService);
        let configuration = new Configuration();
        configuration.Module = stringConstant.taskModule;
        configuration.Status = false;
        configuration.Id = 1;
        let router = fixture.debugElement.injector.get(Router);
        spyOn(router, stringConstant.navigate);
        spyOn(configurationService, stringConstant.updateConfiguration).and.returnValue(new BehaviorSubject({}).asObservable());
        let popup = fixture.debugElement.injector.get(Md2Dialog);
        configurationComponent.openModel(configuration, popup);
        tick();
        expect(router.navigate).toHaveBeenCalled();
    }));

    it('updateConfiguration', fakeAsync(() => {
        let fixture = TestBed.createComponent(ConfigurationComponent); //Create instance of component            
        let configurationComponent = fixture.componentInstance;
        let configurationService = fixture.debugElement.injector.get(ConfigurationService);
        let configuration = new Configuration();
        configuration.Module = stringConstant.taskModule;
        configuration.Status = false;
        configuration.Id = 1;
        let router = fixture.debugElement.injector.get(Router);
        spyOn(router, stringConstant.navigate);
        spyOn(configurationService, stringConstant.updateConfiguration).and.returnValue(new BehaviorSubject({}).asObservable());
        let popup = fixture.debugElement.injector.get(Md2Dialog);
        configurationComponent.updateConfiguration(configuration);
        tick();
        expect(configurationComponent.configuration.Id).toBe(1);
    }));
});