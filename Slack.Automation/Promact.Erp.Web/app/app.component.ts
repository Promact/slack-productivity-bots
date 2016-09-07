
import { Component } from '@angular/core';
@Component({
    selector: 'my-app',
    template: ` 
                <h1>Welcome to Report analysis</h1>
                <h3><a routerLink="/leave" routerLinkActive="active">Leave Reports</a></h3>
                <router-outlet></router-outlet>
`

})
export class AppComponent {
}