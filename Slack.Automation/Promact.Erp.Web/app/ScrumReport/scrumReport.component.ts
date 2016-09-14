import { Component, OnInit }   from '@angular/core';
import { Router, ROUTER_DIRECTIVES}from '@angular/router';
import { ScrumReportService }   from './scrumReport.service';

@Component({
    template: `
    <router-outlet></router-outlet>
`,
    directives: [ROUTER_DIRECTIVES],
    providers: [ ScrumReportService]

})
export class ScrumReportComponent { }