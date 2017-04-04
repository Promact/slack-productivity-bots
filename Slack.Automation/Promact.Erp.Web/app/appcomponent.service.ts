import { Injectable } from '@angular/core';
import { Http } from "@angular/http";
import { StringConstant } from './shared/stringConstant';
import 'rxjs/add/operator/toPromise';

@Injectable()
export class AppComponentService {
    constructor(private http: Http, private stringConstant: StringConstant) { }

    /*
    This service used for get user is Admin or not.*
    */
    getUserIsAdminOrNot() {
        return this.http.get(this.stringConstant.oauthUrl + this.stringConstant.userIsAdmin).map(res => res.json());
    }

    /*
    This service to check is leave app is added for user or not.
    */
    isUserAddedLeaveAppAsync() {
        return this.http.get('api/configuration/isLeaveAppAdded').map(res => res.json());
    }
}