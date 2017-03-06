declare var describe, it, beforeEach, expect, spyOn;
import { async, TestBed, tick, fakeAsync } from '@angular/core/testing';
import { RouterModule, Routes, Router } from '@angular/router';
import { RouterLinkStubDirective } from '../../shared/mock/mock.routerLink';
import { GroupAddComponent } from './groupAdd.component';
import { GroupModule } from '../group.module';
import { GroupService } from '../group.service'
import { StringConstant } from '../../shared/stringConstant';
import { LoaderService } from '../../shared/loader.service';
import { MockRouter } from '../../shared/mock/mock.router';
import { MockGroupService } from '../../shared/mock/mock.group.service';
import { MockToast } from "../../shared/mock/mock.md2toast";
import { GroupModel } from "../group.model";
import { Md2Toast } from 'md2';

let stringConstant = new StringConstant();
describe('Group Add Component Test', () => {
    const routes: Routes = [];

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [RouterLinkStubDirective],
            imports: [GroupModule, RouterModule.forRoot(routes, { useHash: true })],
            providers: [
                { provide: Router, useClass: MockRouter },
                { provide: GroupService, useClass: MockGroupService },
                { provide: StringConstant, useClass: StringConstant },
                { provide: LoaderService, useClass: LoaderService },
                { provide: Md2Toast, useClass: MockToast }
            ]
        }).compileComponents();
    }));

    it("should be defined", () => {
        let fixture = TestBed.createComponent(GroupAddComponent);
        let groupAddComponent = fixture.componentInstance;
        expect(groupAddComponent).toBeDefined();
    });

    it("Ng OnInit", fakeAsync(() => {
        let fixture = TestBed.createComponent(GroupAddComponent);
        let groupAddComponent = fixture.componentInstance;
        groupAddComponent.ngOnInit();
        tick();
        expect(groupAddComponent.listOfActiveEmail).not.toBeNull();
    }));

    it("Add Group", fakeAsync(() => {
        let fixture = TestBed.createComponent(GroupAddComponent);
        let groupAddComponent = fixture.componentInstance;
        let toast = fixture.debugElement.injector.get(Md2Toast);
        let router = fixture.debugElement.injector.get(Router);
        spyOn(router, stringConstant.navigate);
        let groupModel = new GroupModel();
        groupModel.Emails = stringConstant.emails;
        groupModel.Name = stringConstant.testGroupName;
        groupModel.Type = 2;
        groupAddComponent.addGroup(groupModel);
        tick();
        expect(groupModel.Name).toBe(stringConstant.testGroupName);
    }));

    it("ng OnInit Error", fakeAsync(() => {
        let fixture = TestBed.createComponent(GroupAddComponent);
        let groupAddComponent = fixture.componentInstance;
        let toast = fixture.debugElement.injector.get(Md2Toast);
        let router = fixture.debugElement.injector.get(Router);
        spyOn(router, stringConstant.navigate);
        let groupModel = new GroupModel();
        groupModel.Emails = stringConstant.emails;
        groupModel.Name = stringConstant.testGroupName;
        groupModel.Type = 2;
        let groupService = fixture.debugElement.injector.get(GroupService);
        spyOn(groupService, "addGroup").and.returnValue(Promise.reject(""));
        groupAddComponent.addGroup(groupModel);
        tick();
        expect(groupModel).not.toBeNull();
    }));

    it("back To GroupList", () => {
        let fixture = TestBed.createComponent(GroupAddComponent);
        let groupAddComponent = fixture.componentInstance;
        let router = fixture.debugElement.injector.get(Router);
        spyOn(router, stringConstant.navigate);
        groupAddComponent.backToGroupList();
        expect(router.navigate).toHaveBeenCalled();
    });

    it("check GroupName Already Exists Error", fakeAsync(() => {
        let fixture = TestBed.createComponent(GroupAddComponent);
        let groupAddComponent = fixture.componentInstance;
        let groupService = fixture.debugElement.injector.get(GroupService);
        spyOn(groupService, "checkGroupNameIsExists").and.returnValue(Promise.reject(""));
        groupAddComponent.checkGroupName(stringConstant.testGroupName);
        tick();
        expect(stringConstant.testGroupName).toBe(stringConstant.testGroupName);
    }));

    it("check GroupName Already Exists", fakeAsync(() => {
        let fixture = TestBed.createComponent(GroupAddComponent);
        let groupAddComponent = fixture.componentInstance;
        groupAddComponent.checkGroupName(stringConstant.testGroupName);
        tick();
        expect(groupAddComponent.isExistsGroupName).toBe(true);
    }));

    it("check GroupName Already Not Exists", fakeAsync(() => {
        let fixture = TestBed.createComponent(GroupAddComponent);
        let groupAddComponent = fixture.componentInstance;
        groupAddComponent.checkGroupName(stringConstant.groupName);
        tick();
        expect(groupAddComponent.isExistsGroupName).toBe(false);
    }));

});