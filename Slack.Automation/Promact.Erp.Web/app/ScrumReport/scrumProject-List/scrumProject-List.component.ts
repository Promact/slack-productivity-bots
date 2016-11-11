import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ScrumProject } from './scrumProject-List.model';
import { ScrumReportService } from '../scrumReport.service';
import { StringConstant } from '../../shared/stringConstant';

@Component({
    templateUrl: './app/ScrumReport/scrumProject-List/scrumProject-List.html',
})

export class ScrumProjectListComponent implements OnInit {
    scrumProjects: ScrumProject[];
    errorMessage: any;
    noProject: any;

    constructor(private scrumReportService: ScrumReportService, private router: Router, private stringConstant: StringConstant) { }

    ngOnInit() {
        this.getScrumProjects();
    }

    getScrumProjects() {
        
        this.scrumReportService.getScrumProjects()
            .subscribe(
            scrumProjects => {
                this.scrumProjects = scrumProjects;
                if (scrumProjects.length != 0)
                {
                    return scrumProjects;
                }
                else
                {
                    this.noProject = this.stringConstant.noProjectToDisplay;
                    return this.noProject;
                }                    
            },
            error => this.errorMessage = <any>error
        );
    }
}