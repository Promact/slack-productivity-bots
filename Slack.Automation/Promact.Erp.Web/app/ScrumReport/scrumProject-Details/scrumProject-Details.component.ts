import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ScrumDetails } from './scrumProject-Details.model';
import { ScrumReportService } from '../scrumReport.service';
import { StringConstant } from '../../shared/stringConstant';
import { EmployeeScrumAnswers } from './scrumProject-EmployeeScrumDetails.model';

@Component({
    templateUrl: './app/ScrumReport/scrumProject-Details/scrumProject-Details.html',
})


export class ScrumProjectDetailComponent implements OnInit {
    scrumDate: string;
    projectCreationDate: string;
    employeeScrumAnswers: EmployeeScrumAnswers[];
    errorMessage: string;
    Id: number;
    Date: string;
    maxDate = new Date().toISOString().slice(0, 10);
    minDate: string;

    constructor(private scrumReportService: ScrumReportService, private route: ActivatedRoute, private stringConstant: StringConstant ) { }

    ngOnInit() {
        this.getScrumDetailsToday();
    }

    getScrumDetailsToday() {
        this.Date = new Date().toDateString();
        this.getScrumDetails(this.Date);
    }

    getScrumDetailsYesterday(date: string) {
        let yesterday = new Date((new Date(date)).valueOf() - 1000 * 60 * 60 * 24).toDateString();  //subtracting milliseconds in a day
        this.Date = yesterday;
        this.getScrumDetails(this.Date);
    }

    getScrumDetailsGeneral(date: string) {
        this.Date = date;
        this.getScrumDetails(this.Date);
    }

    getScrumDetails(date: string) {
        this.route.params.subscribe(params => this.Id = params[this.stringConstant.paramsId]);
        this.scrumReportService.getScrumDetails(this.Id, date)
            .subscribe(
            (scrumDetails) => {
                this.scrumDate = scrumDetails.ScrumDate;
                this.projectCreationDate = scrumDetails.ProjectCreationDate;
                this.employeeScrumAnswers = scrumDetails.EmployeeScrumAnswers;
                this.minDate = new Date(new Date(scrumDetails.ScrumDate).valueOf() + 1000 * 60 * 60 * 24).toISOString().slice(0, 10);
            },
            error => this.errorMessage = <string>error
            );
    }

    goBack() {
        window.history.back();
    }
}