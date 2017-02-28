import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ScrumProject } from './scrumProject-List.model';
import { ScrumReportService } from '../scrumReport.service';
import { StringConstant } from '../../shared/stringConstant';
import { LoaderService } from '../../shared/loader.service';

@Component({
    templateUrl: './app/ScrumReport/scrumProject-List/scrumProject-List.html',
})

export class ScrumProjectListComponent implements OnInit {
    scrumProjects: ScrumProject[];
    errorMessage: string;
    noProject: string;

    constructor(private scrumReportService: ScrumReportService, private stringConstant: StringConstant, private loader: LoaderService) { }

    ngOnInit() {
        this.getScrumProjects();            
    }

    getScrumProjects() {
        this.loader.loader = true;
        this.scrumReportService.getScrumProjects()
            .subscribe(
            scrumProjects => {
                this.scrumProjects = scrumProjects;
                if (scrumProjects.length !== 0) {
                    this.loader.loader = false;  
                    return scrumProjects;
                }
                else {
                    this.noProject = this.stringConstant.noProjectToDisplay;
                    this.loader.loader = false;  
                    return this.noProject;
                }               
            },
            error => this.errorMessage = <string>error
            );      
    }
}