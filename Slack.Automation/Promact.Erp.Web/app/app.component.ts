
import { Component } from '@angular/core';
@Component({
    selector: 'my-app',
    template: ` 
                <h1>Welcome to leave analysis</h1>
                <h3><a routerLink="report" routerLinkActive="active">Leave Reports</a></h3>
                <h3><a routerLink="task" routerLinkActive="active">Task Reports</a></h3>
                <router-outlet></router-outlet>
`

})
export class AppComponent {
}