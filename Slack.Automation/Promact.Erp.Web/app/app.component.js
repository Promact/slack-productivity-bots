//import { Component } from '@angular/core';
//import { ROUTER_DIRECTIVES } from '@angular/router';
//import { HTTP_PROVIDERS } from '@angular/http';
"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
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
var core_1 = require('@angular/core');
var AppComponent = (function () {
    function AppComponent() {
    }
    AppComponent = __decorate([
        core_1.Component({
            selector: 'my-app',
            template: " \n                <h1>Welcome to leave analysis</h1>\n                <a routerLink=\"/report\" routerLinkActive=\"active\">Leave Reports</a>\n                <router-outlet></router-outlet>\n"
        }), 
        __metadata('design:paramtypes', [])
    ], AppComponent);
    return AppComponent;
}());
exports.AppComponent = AppComponent;
//# sourceMappingURL=app.component.js.map