import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ScrumDetails } from './scrumProject-Details.model';
import { ScrumReportService } from '../scrumReport.service';
import { StringConstant } from '../../shared/stringConstant';
import { EmployeeScrumAnswers } from './scrumProject-EmployeeScrumDetails.model';
import { LoaderService } from '../../shared/loader.service';
import { DatePipe } from '@angular/common';

@Component({
    moduleId: module.id,
    templateUrl: 'scrumProject-Details.html',
})


export class ScrumProjectDetailComponent implements OnInit {
    scrumDate: string;
    projectCreationDate: string;
    employeeScrumAnswers: EmployeeScrumAnswers[];
    errorMessage: string;
    Id: number;
    minDate: string;
    status: boolean;
    minDateForDateRange: Date;
    maxDateForDateRange: Date;
    date: string;

    constructor(private scrumReportService: ScrumReportService, private router: Router, private route: ActivatedRoute, private stringConstant: StringConstant, private loader: LoaderService) {
        this.maxDateForDateRange = new Date();
    }

    ngOnInit() {
        this.getScrumDetailsToday();
    }

    getScrumDetailsToday() {
        this.loader.loader = true;
        let datePipe = new DatePipe(this.stringConstant.medium);
        this.getScrumDetails(datePipe.transform(new Date(), this.stringConstant.dateDefaultFormat));
    }

    getScrumDetailsYesterday(date: string) {
        this.loader.loader = true;
        let datePipe = new DatePipe(this.stringConstant.medium);
        let currentDate = new Date(date);
        let yesterDaty = datePipe.transform(new Date(currentDate.setDate(currentDate.getDate() - 1)), this.stringConstant.dateDefaultFormat);
        this.getScrumDetails(yesterDaty);
    }

    getScrumDetailsGeneral(date: string) {
        this.loader.loader = true;
        let datePipe = new DatePipe(this.stringConstant.medium);
        this.getScrumDetails(datePipe.transform(date, this.stringConstant.dateDefaultFormat));
    }

    getScrumDetails(date: string) {
        this.route.params.subscribe(params => this.Id = params[this.stringConstant.paramsId]);
        this.scrumReportService.getScrumDetails(+this.Id, date)
            .subscribe(
            (scrumDetails) => {
                this.status = false;
                this.scrumDate = scrumDetails.ScrumDate;
                this.projectCreationDate = scrumDetails.ProjectCreationDate;
                this.minDateForDateRange = new Date(this.projectCreationDate);
                this.employeeScrumAnswers = scrumDetails.EmployeeScrumAnswers;
                if (this.employeeScrumAnswers !== null && this.employeeScrumAnswers !== undefined) {
                    for (let index = 0; index < this.employeeScrumAnswers.length; index++) {
                        let answer = this.employeeScrumAnswers[index];
                        answer.Answer1 = this.replaceSpecialCharacter(answer.Answer1);
                        answer.Answer2 = this.replaceSpecialCharacter(answer.Answer2);
                        answer.Answer3 = this.replaceSpecialCharacter(answer.Answer3);
                    }
                }
                this.minDate = new Date(new Date(scrumDetails.ScrumDate).valueOf() + 1000 * 60 * 60 * 24).toISOString().slice(0, 10);
                if (scrumDetails.EmployeeScrumAnswers === null) {this.status = true;}
                this.loader.loader = false;
            },
            error => this.errorMessage = <string>error
            );
    }

    goBack() {
        this.router.navigate([this.stringConstant.scrumList]);
    }

    replaceSpecialCharacter(value: Array<string>) {
        for (let index = 0; index < value.length; index++) {
            value[index] = value[index].replace('&amp;', '&').replace('&lt;', '<').replace('&gt;', '>');
        }
        return value;
    }
}