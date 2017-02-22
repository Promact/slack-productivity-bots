import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MailSetting } from './mailsetting.model';
import { NgModule } from "@angular/core";


@Component({
    templateUrl: './app/shared/MailSetting/mailsetting.html',
})
export class MailSettingComponent implements OnInit {
    mailSetting: MailSetting;
    projectList: Array<string>;
    currentLocation: string;
    SendMail: string;
    constructor() { }
    ngOnInit() {
        this.SendMail = "test";
        //this.projectList = [];
    }

    //getCurrentLocationAndProvideModule() {
    //    this.currentLocation = window.location.hash;
    //    let listofString = this.currentLocation.split('/');
    //    return listofString[1];
    //}

    //addMailSetting(mailSetting: MailSetting) {
    //    mailSetting.Module = this.getCurrentLocationAndProvideModule();
    //}
}