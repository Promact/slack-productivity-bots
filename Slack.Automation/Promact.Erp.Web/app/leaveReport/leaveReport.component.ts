import { Component, OnInit }   from '@angular/core';
import { Router, ROUTER_DIRECTIVES}from '@angular/router';
import { LeaveReportService }   from './leaveReport.service';

@Component({
    template: `
    <router-outlet></router-outlet>
`,
    directives: [ROUTER_DIRECTIVES],
    providers: [LeaveReportService]

})
export class LeaveReportComponent { }