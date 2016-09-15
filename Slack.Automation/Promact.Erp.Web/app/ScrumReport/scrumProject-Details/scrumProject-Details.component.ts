import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { ScrumDetails } from './scrumProject-Details.model';
import { ScrumReportService } from '../scrumReport.service';
import { StringConstant } from '../../shared/stringConstant';

@Component({
    templateUrl: './app/ScrumReport/scrumProject-Details/scrumProject-Details.html',
})



export class ScrumProjectDetailComponent implements OnInit {
    scrumDetails: ScrumDetails[] = [];
    errorMessage: string;
    sub: any;
    Id: any;
    Date: any;
    maxDate = new Date().toISOString().slice(0, 10);

    constructor(private scrumReportService: ScrumReportService, private route: ActivatedRoute, private stringConstant: StringConstant) { }

    ngOnInit() {
        this.getScrumDetailsToday();
    }

    getScrumDetailsToday() {
        this.Date = new Date().toDateString();
        this.getScrumDetails(this.Date);
    }

    getScrumDetailsYesterday(date: any) {
        var yesterday = new Date((new Date(date)).valueOf() - 1000 * 60 * 60 * 24).toDateString();  //subtracting milliseconds in a day
        this.Date = yesterday;
        this.getScrumDetails(this.Date);
    }

    getScrumDetailsGeneral(date: any) {
        this.Date = date;
        this.getScrumDetails(this.Date);
    }

    getScrumDetails(date: any) {
        this.sub = this.route.params.subscribe(params => this.Id = params[this.stringConstant.paramsId]);
        this.scrumReportService.getScrumDetails(this.Id, date)
            .subscribe(
            scrumDetails =>  this.scrumDetails = scrumDetails,               
            error => this.errorMessage = <any>error
            );
    }


    goBack() {
        window.history.back();
    }  
}
