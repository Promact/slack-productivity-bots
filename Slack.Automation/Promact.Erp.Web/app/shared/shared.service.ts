import { Injectable } from '@angular/core';
import 'rxjs/add/operator/toPromise';
import { ConfigurationStatusAC } from '../configuration/configuration.model';
import { Subject } from "rxjs/Rx";

@Injectable()
export class SharedService {
    public configurationStatus = new Subject<ConfigurationStatusAC>();
    public status = new ConfigurationStatusAC();
    
    constructor() { }

    setConfigurationStatusAC(configuration: ConfigurationStatusAC) {
        this.configurationStatus.next(configuration);
        this.status = configuration;
    }

    getConfigurationStatusAC() {
        return this.status;
    }
}