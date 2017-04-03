declare var describe, it, beforeEach, expect;
import { async, TestBed } from '@angular/core/testing';
import { RouterModule, Routes } from '@angular/router';
import { RouterLinkStubDirective } from '../shared/mock/mock.routerLink';
import { GroupComponent } from './group.component';
import { GroupModule } from './group.module';

describe('Group Component Test', () => {
    const routes: Routes = [];

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [RouterLinkStubDirective],
            imports: [GroupModule, RouterModule.forRoot(routes, { useHash: true })],
            providers: []
        }).compileComponents();
    }));

    it("should be defined", () => {
        let fixture = TestBed.createComponent(GroupComponent);
        let groupComponent = fixture.componentInstance;
        expect(groupComponent).toBeDefined();
    });
});