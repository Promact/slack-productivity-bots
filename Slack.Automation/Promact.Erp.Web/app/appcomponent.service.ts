import { Injectable } from '@angular/core';
import { Http } from "@angular/http";
import 'rxjs/add/operator/toPromise';

@Injectable()
export class AppComponentService {
    private oauthControllerUrl = 'oauth/';  // URL to web api
    constructor(private http: Http) { }

    /*This service used for get user is Admin or not.*
    */
    getUserIsAdminOrNot() {
        return this.http.get(this.oauthControllerUrl + 'userIsAdmin').map(res => res.text()).toPromise();
    }
}