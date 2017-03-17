
import { Component, OnInit } from '@angular/core';
import { LoaderService } from './shared/loader.service';
import { AppComponentService } from './appcomponent.service';
import { EmailHashCode } from './shared/emailHashCode';
import { ConfigurationService } from './configuration/configuration.service';
import { ConfigurationStatusAC } from './configuration/configuration.model';
import { SharedService } from './shared/shared.service';

@Component({
    selector: 'my-app',
    templateUrl: './app/index.html'

})

export class AppComponent implements OnInit {
    userIsAdmin: boolean;
    hashCode: string;
    username: string;
    private configurationStatus: ConfigurationStatusAC = new ConfigurationStatusAC();
    constructor(private loader: LoaderService, private httpService: AppComponentService, private emailHashCode: EmailHashCode, private configurationHttpService: ConfigurationService, private sharedService: SharedService) {
        this.userIsAdmin = false;
    }

    ngOnInit() {
        this.sharedService.configurationStatus.subscribe((state) => {
            setTimeout(() => {
                this.configurationStatus = state;
            }, 0);
        });

        this.loader.loader = true;
        this.hashCode = this.emailHashCode.hashCode;
        this.configurationHttpService.getListOfConfigurationStatus().then((result) => {
            this.configurationStatus = result;
            this.sharedService.setConfigurationStatusAC(this.configurationStatus);
        });
        this.httpService.getUserIsAdminOrNot().subscribe((result) => {
            this.userIsAdmin = result.IsAdmin;
            this.username = result.FirstName;
        });
        this.loader.loader = false;
    }
}