import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { ScrumProject } from './scrumProject-List.model';
import { ScrumReportService } from '../scrumReport.service';

@Component({
    templateUrl: './app/ScrumReport/scrumProject-List/scrumProject-List.html',
})

export class ScrumProjectListComponent implements OnInit {
    scrumProjects: ScrumProject[];
    errorMessage: any;

    constructor(private scrumReportService: ScrumReportService, private router: Router) { }

    ngOnInit() {
        this.getScrumProjects();
    }

    getScrumProjects() {
        this.scrumReportService.getScrumProjects()
            .subscribe(
            scrumProjects => this.scrumProjects = scrumProjects,
            error => this.errorMessage = <any>error
        );
    }
}