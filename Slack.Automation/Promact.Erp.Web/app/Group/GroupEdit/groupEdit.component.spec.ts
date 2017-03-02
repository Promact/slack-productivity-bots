declare var describe, it, beforeEach, expect, spyOn;
import { async, TestBed, tick, fakeAsync } from '@angular/core/testing';
import { RouterModule, Routes, Router, ActivatedRoute } from '@angular/router';
import { RouterLinkStubDirective } from '../../shared/mock/mock.routerLink';
import { GroupEditComponent } from './groupEdit.component';
import { GroupModule } from '../group.module';
import { GroupService } from '../group.service'
import { StringConstant } from '../../shared/stringConstant';
import { LoaderService } from '../../shared/loader.service';
import { MockRouter } from '../../shared/mock/mock.router';
import { MockGroupService } from '../../shared/mock/mock.group.service';
import { MockToast } from "../../shared/mock/mock.md2toast";
import { GroupModel } from "../group.model";
import { ActivatedRouteStub } from "../../shared/mock/mock.activatedroute";
import { Md2Toast } from 'md2';

let stringConstant = new StringConstant();
describe('Group Edit Component Test', () => {
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
                { provide: Md2Toast, useClass: MockToast },
                { provide: ActivatedRoute, useClass: ActivatedRouteStub },
            ]
        }).compileComponents();
    }));

    it("should be defined", () => {
        let fixture = TestBed.createComponent(GroupEditComponent);
        let groupEditComponent = fixture.componentInstance;
        expect(groupEditComponent).toBeDefined();
    });

    it("ng OnInit", fakeAsync(() => {
        let fixture = TestBed.createComponent(GroupEditComponent);
        let activatedRoute = fixture.debugElement.injector.get(ActivatedRoute);
        activatedRoute.testParams = { id: stringConstant.id };
        let groupEditComponent = fixture.componentInstance;
        groupEditComponent.ngOnInit();
        tick();
        expect(groupEditComponent.groupModel).not.toBeNull();
    }));

    it("check GroupName Already Exists", fakeAsync(() => {
        let fixture = TestBed.createComponent(GroupEditComponent);
        let groupEditComponent = fixture.componentInstance;
        groupEditComponent.checkGroupName(stringConstant.testGroupName);
        tick();
        expect(groupEditComponent.isExistsGroupName).toBe(true);
    }));

    it("check GroupName Already Not Exists", fakeAsync(() => {
        let fixture = TestBed.createComponent(GroupEditComponent);
        let groupEditComponent = fixture.componentInstance;
        groupEditComponent.checkGroupName(stringConstant.groupName);
        tick();
        expect(groupEditComponent.isExistsGroupName).toBe(false);
    }));

    it("back To GroupList", () => {
        let fixture = TestBed.createComponent(GroupEditComponent);
        let groupEditComponent = fixture.componentInstance;
        let router = fixture.debugElement.injector.get(Router);
        spyOn(router, stringConstant.navigate);
        groupEditComponent.backToGroupList();
        expect(router.navigate).toHaveBeenCalled();
    });

    it("Update Group", fakeAsync(() => {
        let fixture = TestBed.createComponent(GroupEditComponent);
        let groupEditComponent = fixture.componentInstance;
        let toast = fixture.debugElement.injector.get(Md2Toast);
        let router = fixture.debugElement.injector.get(Router);
        spyOn(router, stringConstant.navigate);
        let groupModel = new GroupModel();
        groupModel.Emails = stringConstant.emails;
        groupModel.Name = stringConstant.testGroupName;
        groupModel.Type = 2;
        groupEditComponent.updateGroup(groupModel);
        tick();
        expect(groupModel.Name).toBe(stringConstant.testGroupName);
    }));

});