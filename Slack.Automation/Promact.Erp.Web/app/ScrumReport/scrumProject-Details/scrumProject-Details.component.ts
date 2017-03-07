import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ScrumDetails } from './scrumProject-Details.model';
import { ScrumReportService } from '../scrumReport.service';
import { StringConstant } from '../../shared/stringConstant';
import { EmployeeScrumAnswers } from './scrumProject-EmployeeScrumDetails.model';
import { LoaderService } from '../../shared/loader.service';
import { DatePipe } from '@angular/common';

@Component({
    templateUrl: './app/ScrumReport/scrumProject-Details/scrumProject-Details.html',
})


export class ScrumProjectDetailComponent implements OnInit {
    scrumDate: string;
    projectCreationDate: string;
    employeeScrumAnswers: EmployeeScrumAnswers[];
    errorMessage: string;
    Id: number;
    maxDate = new Date().toISOString().slice(0, 10);
    minDate: string;
    
    constructor(private scrumReportService: ScrumReportService, private router: Router,private route: ActivatedRoute, private stringConstant: StringConstant, private loader: LoaderService ) { }
    
    ngOnInit() {
        this.getScrumDetailsToday();
    }

    getScrumDetailsToday() {
        let datePipe = new DatePipe(this.stringConstant.medium);
        this.getScrumDetails(datePipe.transform(new Date(), this.stringConstant.dateDefaultFormat));
    }

    getScrumDetailsYesterday(date: string) {
        let datePipe = new DatePipe(this.stringConstant.medium);
        let currentDate = new Date(date);
        let yesterDaty = datePipe.transform(new Date(currentDate.setDate(currentDate.getDate() - 1)), this.stringConstant.dateDefaultFormat);
        this.getScrumDetails(yesterDaty);
    }

    getScrumDetailsGeneral(date: string) {
        let datePipe = new DatePipe(this.stringConstant.medium);
        this.getScrumDetails(datePipe.transform(date, this.stringConstant.dateDefaultFormat));
    }

    getScrumDetails(date: string) {
        this.loader.loader = true;
        this.route.params.subscribe(params => this.Id = params[this.stringConstant.paramsId]); 
        this.scrumReportService.getScrumDetails(+this.Id,date)
            .subscribe(
            (scrumDetails) => {
                this.scrumDate = scrumDetails.ScrumDate;
                this.projectCreationDate = scrumDetails.ProjectCreationDate;
                this.employeeScrumAnswers = scrumDetails.EmployeeScrumAnswers;
                this.minDate = new Date(new Date(scrumDetails.ScrumDate).valueOf() + 1000 * 60 * 60 * 24).toISOString().slice(0, 10);
                this.loader.loader = false;
            },
            error => this.errorMessage = <string>error
            );        
    }

    goBack() {
        this.router.navigate([this.stringConstant.scrumList]);
    }
}