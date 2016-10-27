import { Component, OnInit }   from '@angular/core';
import { Router }from '@angular/router';
import { LeaveReportService }   from './leaveReport.service';

@Component({
    template: `
    <router-outlet></router-outlet>
`,
    providers: [LeaveReportService]

})
export class LeaveReportComponent { }