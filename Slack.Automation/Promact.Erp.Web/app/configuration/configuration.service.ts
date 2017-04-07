import { Injectable } from '@angular/core';
import { Http, Headers } from "@angular/http";
import 'rxjs/add/operator/toPromise';
import { URLSearchParams } from '@angular/http';
import { Configuration } from './configuration.model';

@Injectable()
export class ConfigurationService {
    private configurationUrl = 'api/configuration';
    private headers = new Headers({ 'Content-Type': 'application/json' });
    constructor(private http: Http) {
    }

    /*This service used for get configuration list*
    *
    */
    getListOfConfiguration() {
        return this.http.get(this.configurationUrl).map(res => res.json());
    }

    /*This service used for add new configuration*
    * /
    * @param configuration
     */
    updateConfiguration(configuration: Configuration) {
        return this.http
            .put(this.configurationUrl, JSON.stringify(configuration), { headers: this.headers });
    }

    /*This service used for get configuration list*
    *
    */
    getListOfConfigurationStatus() {
        return this.http.get(this.configurationUrl + '/status').map(res => res.json());
    }
}