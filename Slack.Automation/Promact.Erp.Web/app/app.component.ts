
import { Component } from '@angular/core';
@Component({
    selector: 'my-app',
    template: ` 
                <h1>Welcome to leave analysis</h1>
                <a routerLink="/report" routerLinkActive="active">Leave Reports</a>
                <router-outlet></router-outlet>
`

})
export class AppComponent {
}