import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ScrumProject } from './scrumProject-List.model';
import { ScrumReportService } from '../scrumReport.service';
import { StringConstant } from '../../shared/stringConstant';
import { LoaderService } from '../../shared/loader.service';

@Component({
    moduleId: module.id,
    templateUrl: 'scrumProject-List.html',
})

export class ScrumProjectListComponent implements OnInit {
    scrumProjects: ScrumProject[];
    errorMessage: string;
    noProject: boolean;


    constructor(private scrumReportService: ScrumReportService, private stringConstant: StringConstant, private loader: LoaderService) {
        this.noProject = false;
    }

    ngOnInit() {
        this.getScrumProjects();
    }
    
    getScrumProjects() {
        this.loader.loader = true;
        this.scrumReportService.getScrumProjects().subscribe(result => {
            if (result.length > 0) {
                this.scrumProjects = result;
                this.loader.loader = false;
            }
            else {
                this.noProject = true;
                this.loader.loader = false;
            }
        },
            error => this.errorMessage = <string>error
        );
    }
}