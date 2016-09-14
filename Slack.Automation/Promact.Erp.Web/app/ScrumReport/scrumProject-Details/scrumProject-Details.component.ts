import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { ScrumDetails } from './scrumProject-Details.model';
import { ScrumReportService } from '../scrumReport.service';

@Component({
    templateUrl: './app/ScrumReport/scrumProject-Details/scrumProject-Details.html',
})

export class ScrumProjectDetailComponent implements OnInit {
    scrumDetails: ScrumDetails[] = [];
    errorMessage: string;
    sub: any;
    Id: any;
    Date: any;

    constructor(private scrumReportService: ScrumReportService, private route: ActivatedRoute) { }

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
        this.sub = this.route.params.subscribe(params => this.Id = params['id']);
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