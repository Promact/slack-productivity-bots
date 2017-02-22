import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MailSetting } from './mailsetting.model';
import { NgModule } from "@angular/core";
import { Project } from './project.model';

@Component({
    templateUrl: './app/shared/MailSetting/mailsetting.html',
})
export class MailSettingComponent implements OnInit {
    mailSettinglist: Array<Project>;
    emailList: Array<string>;
    currentLocation: string;
    constructor() { }
    ngOnInit() {
        this.emailList = new Array<string>();
        this.emailList = ["hello", "bye"];
    }

    getCurrentLocationAndProvideModule() {
        this.currentLocation = window.location.hash;
        let listofString = this.currentLocation.split('/');
        return listofString[1];
    }

    addMailSetting(mailSetting: MailSetting) {
        mailSetting.Module = this.getCurrentLocationAndProvideModule();
    }
}