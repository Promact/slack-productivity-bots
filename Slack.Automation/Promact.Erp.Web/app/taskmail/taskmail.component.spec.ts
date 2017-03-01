declare var describe, it, beforeEach, expect;
import { async, TestBed } from '@angular/core/testing';
import { RouterModule, Routes } from '@angular/router';
import { RouterLinkStubDirective } from '../shared/mock/mock.routerLink';
import { TaskMailComponent } from './taskmail.component';
import { AppModule } from '../app.module';

describe('Task Mail Component Test', () => {
    const routes: Routes = [];

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [RouterLinkStubDirective],
            imports: [AppModule,RouterModule.forRoot(routes, { useHash: true })],
            providers: []
        }).compileComponents();
    }));

    it("should be defined", () => {
        let fixture = TestBed.createComponent(TaskMailComponent);
        let taskMailComponent = fixture.componentInstance;
        expect(taskMailComponent).toBeDefined();
    });
});
