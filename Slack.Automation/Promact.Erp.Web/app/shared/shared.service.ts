import { Injectable } from '@angular/core';
import { Http, Headers } from "@angular/http";
import 'rxjs/add/operator/toPromise';
import { URLSearchParams } from '@angular/http';
import { ConfigurationStatusAC } from '../configuration/configuration.model';
import { Subject } from "rxjs/Rx";

@Injectable()
export class SharedService {
    public configurationStatus = new Subject<ConfigurationStatusAC>();
    
    constructor() { }

    setConfigurationStatusAC(configuration: ConfigurationStatusAC) {
        this.configurationStatus.next(configuration);
    }
}