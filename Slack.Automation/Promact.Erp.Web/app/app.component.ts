
import { Component, OnInit, ViewChild } from '@angular/core';
import { LoaderService } from './shared/loader.service';
import { AppComponentService } from './appcomponent.service';
import { EmailHashCode } from './shared/emailHashCode';
import { ConfigurationService } from './configuration/configuration.service';
import { ConfigurationStatusAC } from './configuration/configuration.model';
import { SharedService } from './shared/shared.service';
import { Md2Toast, Md2DialogConfig } from 'md2';

@Component({
    selector: 'my-app',
    templateUrl: './app/index.html'

})

export class AppComponent implements OnInit {
    userIsAdmin: boolean;
    hashCode: string;
    username: string;
    dialogConfig: Md2DialogConfig = { disableClose: true };
    leaveConfigurationId: number;
    private configurationStatus: ConfigurationStatusAC = new ConfigurationStatusAC();
    constructor(private loader: LoaderService, private httpService: AppComponentService,
        private emailHashCode: EmailHashCode, private configurationHttpService: ConfigurationService,
        private sharedService: SharedService) {
        this.userIsAdmin = false;
    }

    @ViewChild('confirm') model: any;

    ngOnInit() {
        this.sharedService.configurationStatus.subscribe((state) => {
            setTimeout(() => {
                this.configurationStatus = state;
            }, 0);
        });

        this.loader.loader = true;
        this.hashCode = this.emailHashCode.hashCode;
        this.configurationHttpService.getListOfConfigurationStatus().subscribe((result) => {
            this.configurationStatus = result;
            this.sharedService.setConfigurationStatusAC(this.configurationStatus);
            if (this.configurationStatus.LeaveOn) {
                this.httpService.isUserAddedLeaveAppAsync().subscribe((res) => {
                    if (!res.IsAdded) {
                        this.leaveConfigurationId = res.ConfigurationId;
                        this.model.open(this.dialogConfig);
                    }
                });
            }
        });
        this.httpService.getUserIsAdminOrNot().subscribe((result) => {
            this.userIsAdmin = result.IsAdmin;
            this.username = result.FirstName;
            this.loader.loader = false;
        });
    }

    AddToSlack() {
        window.location.href = '/Home/SlackOAuthAuthorization?configurationId=' + this.leaveConfigurationId;
    }
}