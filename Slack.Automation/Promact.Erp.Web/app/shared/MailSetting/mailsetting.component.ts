import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MailSetting } from './mailsetting.model';
import { NgModule } from "@angular/core";
import { Project } from './project.model';
import { MailSettingService } from './mailsetting.service';
import { LoaderService } from '../../shared/loader.service';
import { Md2Toast } from 'md2';
import { MailSettingAC } from './mailsettingAC.model';
import { StringConstant } from '../stringConstant';

@Component({
    moduleId: module.id,
    templateUrl: 'mailsetting.html',
})
export class MailSettingComponent implements OnInit {
    mailSetting: MailSetting = new MailSetting;
    listOfProject: Array<Project>;
    groupList: Array<string>;
    isToUpdate: boolean;
    selectedMailSetting: MailSetting = new MailSetting;
    showButton: boolean;
    mailSettingAC: MailSettingAC;
    currentModule: string;
    projectSelected: boolean;

    constructor(private httpService: MailSettingService, private loader: LoaderService, private router: Router,
        private toaster: Md2Toast, private stringConstant: StringConstant) {
        let currentLocation = window.location.hash;
        let listofString = currentLocation.split('/');
        this.currentModule = listofString[1];
        this.showButton = false;
        this.groupList = new Array<string>();
        this.mailSettingAC = new MailSettingAC;
    }

    ngOnInit() {
        this.loader.loader = true;
        this.getGroups();
        this.getAllProject();
        this.projectSelected = false;
    };

    addMailSetting(mailSetting: MailSetting) {
        this.loader.loader = true;
        this.mailSettingAC.CC = mailSetting.CC;
        this.mailSettingAC.ProjectId = mailSetting.Project.Id;
        this.mailSettingAC.Module = this.currentModule;
        this.mailSettingAC.SendMail = mailSetting.SendMail;
        this.mailSettingAC.To = mailSetting.To;
        this.httpService.addMailSetting(this.mailSettingAC).subscribe((result) => {
            this.toaster.show(this.stringConstant.mailSettingOf + ' ' + this.currentModule + ' ' + this.stringConstant.successfully + ' ' + this.stringConstant.added);
            this.router.navigate(['/']);
        });
        this.loader.loader = false;
    };

    updateMailSetting(mailSetting: MailSetting) {
        this.loader.loader = true;
        this.mailSettingAC.CC = mailSetting.CC;
        this.mailSettingAC.ProjectId = mailSetting.Project.Id;
        this.mailSettingAC.Module = this.currentModule;
        this.mailSettingAC.SendMail = mailSetting.SendMail;
        this.mailSettingAC.To = mailSetting.To;
        this.mailSettingAC.Id = mailSetting.Id;
        this.httpService.updateMailSetting(this.mailSettingAC).subscribe((result) => {
            this.toaster.show(this.stringConstant.mailSettingOf + ' ' + this.currentModule + ' ' + this.stringConstant.successfully + ' ' + this.stringConstant.updated);
            this.router.navigate(['/']);
            this.loader.loader = false;
        });
    };

    getAllProject() {
        this.httpService.getAllProjects().subscribe((result) => {
            this.listOfProject = result;
        });
        this.loader.loader = false;
    };

    getMailSettingDetailsByProjectId(Id: number) {
        this.loader.loader = true;
        this.projectSelected = true;
        this.httpService.getProjectByIdAndModule(Id, this.currentModule).subscribe((result) => {
            this.selectedMailSetting = result;
            this.mailSetting.CC = this.selectedMailSetting.CC;
            this.mailSetting.To = this.selectedMailSetting.To;
            this.mailSetting.Id = this.selectedMailSetting.Id;
            this.mailSetting.Module = this.selectedMailSetting.Module;
            this.mailSetting.SendMail = this.selectedMailSetting.SendMail;
            if (this.selectedMailSetting.Id === 0) {
                this.isToUpdate = false;
            }
            else {
                this.isToUpdate = true;
            }
        });
        this.showButton = true;
        this.loader.loader = false;
    }

    getGroups() {
        this.httpService.getListOfGroups().subscribe((result) => {
            this.groupList = result;
        });
    };
}