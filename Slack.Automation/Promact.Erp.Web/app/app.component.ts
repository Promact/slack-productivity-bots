//import { Component } from '@angular/core';
//import { ROUTER_DIRECTIVES } from '@angular/router';
//import { HTTP_PROVIDERS } from '@angular/http';


//@Component({
//    selector: 'my-app',
//    template: `<h3>{{title}}<h3>
//              <h1>Welcome to Leave Analysis</h1>
//              <a [routerLink]= "['/report']" routerLinkActive="active">Show me the report</a></nav>                 
//              <router-outlet></router-outlet>  
//           `,
//    directives: [ROUTER_DIRECTIVES]
//})


//export class AppComponent  {
//        title : 'xyz'
//}


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