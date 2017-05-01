import { Component, OnInit }   from '@angular/core';
import { Router }from '@angular/router';
import { ScrumReportService }   from './scrumReport.service';

@Component({
    moduleId: module.id,
    template: `
    <router-outlet></router-outlet>
`,
    providers: [ ScrumReportService]

})
export class ScrumReportComponent { }