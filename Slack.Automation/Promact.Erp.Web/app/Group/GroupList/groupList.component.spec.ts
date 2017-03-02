declare var describe, it, beforeEach, expect, spyOn;
import { async, TestBed, tick, fakeAsync } from '@angular/core/testing';
import { RouterModule, Routes, Router } from '@angular/router';
import { RouterLinkStubDirective } from '../../shared/mock/mock.routerLink';
import { GroupListComponent } from './groupList.component';
import { GroupModule } from '../group.module';
import { GroupService } from '../group.service'
import { StringConstant } from '../../shared/stringConstant';
import { LoaderService } from '../../shared/loader.service';
import { MockRouter } from '../../shared/mock/mock.router';
import { MockGroupService } from '../../shared/mock/mock.group.service';
import { Md2Dialog, Md2Toast } from 'md2';
import { MockToast } from "../../shared/mock/mock.md2toast";
import { MockDialog } from "../../shared/mock/mock.md2Dialog";

let stringConstant = new StringConstant();

describe('Group List Component Test', () => {
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
                { provide: Md2Dialog, useClass: MockDialog }
            ]
        }).compileComponents();
    }));

    it("should be defined", () => {
        let fixture = TestBed.createComponent(GroupListComponent);
        let groupListComponent = fixture.componentInstance;
        expect(groupListComponent).toBeDefined();
    });

    it("ngOnInit", fakeAsync(() => {
        let fixture = TestBed.createComponent(GroupListComponent); //Create instance of component            
        let groupListComponent = fixture.componentInstance;
        groupListComponent.ngOnInit();
        tick();
        expect(groupListComponent.groupList).not.toBeNull();
    }));

    it("add New", () => {
        let fixture = TestBed.createComponent(GroupListComponent); //Create instance of component            
        let groupListComponent = fixture.componentInstance;
        let router = fixture.debugElement.injector.get(Router);
        spyOn(router, stringConstant.navigate);
        groupListComponent.addNewGroup();
        expect(router.navigate).toHaveBeenCalled();
    });


    it("edit Group", () => {
        let fixture = TestBed.createComponent(GroupListComponent); //Create instance of component            
        let groupListComponent = fixture.componentInstance;
        let router = fixture.debugElement.injector.get(Router);
        spyOn(router, stringConstant.navigate);
        groupListComponent.editGroup(1);
        expect(router.navigate).toHaveBeenCalled();
    });

    it("delte GroupPopup", fakeAsync(() => {
        let fixture = TestBed.createComponent(GroupListComponent); //Create instance of component            
        let groupListComponent = fixture.componentInstance;
        let popup = fixture.debugElement.injector.get(Md2Dialog);
        groupListComponent.delteGroupPopup(1, popup);
        tick();
        expect(groupListComponent.groupId).toBe(1);

    }));

    it("delete Group", () => {
        let fixture = TestBed.createComponent(GroupListComponent); //Create instance of component            
        let groupListComponent = fixture.componentInstance;
        let popup = fixture.debugElement.injector.get(Md2Dialog);
        groupListComponent.deleteGroup(popup);
        expect(groupListComponent.groupList).toEqual(undefined);
    });

    it("closeDelete Popup", () => {
        let fixture = TestBed.createComponent(GroupListComponent); //Create instance of component            
        let groupListComponent = fixture.componentInstance;
        let popup = fixture.debugElement.injector.get(Md2Dialog);
        groupListComponent.closeDeletePopup(popup);
    });
});