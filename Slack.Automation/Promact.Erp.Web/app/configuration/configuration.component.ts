import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgModule } from "@angular/core";
import { LoaderService } from '../shared/loader.service';
import { Md2Toast } from 'md2';
import { ConfigurationService } from './configuration.service';
import { Configuration, ConfigurationStatusAC } from './configuration.model';
import { SharedService } from '../shared/shared.service';

@Component({
    templateUrl: './app/configuration/configuration.html',
})
export class ConfigurationComponent implements OnInit {
    private configurationStatus: ConfigurationStatusAC = new ConfigurationStatusAC();
    private configurationList: Array<Configuration> = new Array<Configuration>();
    private configurationId: number;
    private configuration: Configuration = new Configuration();
    constructor(private httpService: ConfigurationService, private router: Router, private loader: LoaderService, private sharedService: SharedService) { }

    ngOnInit() {
        this.loader.loader = true;
        this.httpService.getListOfConfiguration().then((result) => {
            this.configurationList = result;
        });
        this.loader.loader = false;
    }

    openModel(configuration: Configuration, popup) {
        this.loader.loader = true;
        this.configurationId = configuration.Id;
        this.updateConfiguration(configuration);
        this.loader.loader = false;
        if (configuration.Status === true) {
            popup.open();
        }
        else {
            this.loader.loader = true;
            if (configuration.Module === "leave")
            { this.configurationStatus.LeaveOn = configuration.Status; }
            if (configuration.Module === "task")
            { this.configurationStatus.TaskOn = configuration.Status; }
            if (configuration.Module === "scrum")
            { this.configurationStatus.ScrumOn = configuration.Status; }
            this.sharedService.setConfigurationStatusAC(this.configurationStatus);
            this.loader.loader = false;
            this.router.navigate(['/'])
        }
    }

    AddToSlack(popup) {
        popup.close();
        window.location.href = '/Home/SlackOAuthAuthorization/' + this.configurationId
    }

    updateConfiguration(configuration: Configuration) {
        this.configuration = configuration;
        this.httpService.updateConfiguration(configuration).subscribe((result) => {
        })
    }
}