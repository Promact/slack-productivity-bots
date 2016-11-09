﻿
import { Component } from '@angular/core';
//import { SpinnerComponent} from './shared/spinner.component';
@Component({
    selector: 'my-app',
    //directives: [SpinnerComponent],
    template: ` 
                <h1>Welcome to Reports analysis</h1>
                <spinner-component></spinner-component>
                <h3><a routerLink="/leave" routerLinkActive="active">Leave Reports</a></h3>
                <h3><a routerLink="task" routerLinkActive="active">Task Reports</a></h3>
                 <h3><a routerLink="/scrum" routerLinkActive="active">Scrum Reports</a></h3>
                <router-outlet></router-outlet>
`

})
export class AppComponent {
}