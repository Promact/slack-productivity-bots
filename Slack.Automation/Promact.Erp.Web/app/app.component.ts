
import { Component } from '@angular/core';
import { LoaderService } from './shared/loader.service'
@Component({
    selector: 'my-app',
    //directives: [SpinnerComponent],
    template: ` 
                <h1>Welcome to leave analysis</h1>
                <spinner-component></spinner-component>
                <h3><a routerLink="/leave" routerLinkActive="active">Leave Reports</a></h3>
                <h3><a routerLink="task" routerLinkActive="active">Task Reports</a></h3>
 
                <router-outlet></router-outlet>
`

})
    
export class AppComponent {
    constructor(private loader: LoaderService) { }
}