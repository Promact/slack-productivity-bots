import { Component, OnInit }   from '@angular/core';
import { Router}from '@angular/router';
import { TaskService }   from './taskmail.service';

@Component({
    template: `
    <router-outlet></router-outlet>
`,
    providers: [TaskService]

})
export class TaskMailComponent { }