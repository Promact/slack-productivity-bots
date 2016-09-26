import { Component, OnInit }   from '@angular/core';
import { Router, ROUTER_DIRECTIVES}from '@angular/router';
import { TaskService }   from './taskmail.service';

@Component({
    template: `
    <router-outlet></router-outlet>
`,
    directives: [ROUTER_DIRECTIVES],
    providers: [TaskService]

})
export class TaskMailComponent { }