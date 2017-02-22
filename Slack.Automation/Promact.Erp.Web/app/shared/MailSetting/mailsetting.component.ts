import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MailSetting } from './mailsetting.model';

@Component({
    templateUrl: './app/shared/MailSetting/mailsetting.html',
})
export class MailSettingComponent implements OnInit {
    currentLocation: string;
    constructor() { }
    ngOnInit() {

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