import { Injectable } from '@angular/core';
import { Http, Headers } from "@angular/http";
import 'rxjs/add/operator/toPromise';
import { MailSetting } from './mailsetting.model';
import { MailSettingAC } from './mailsettingAC.model';

@Injectable()
export class MailSettingService {
    private mailSettingUrl = 'api/mailsetting';
    private headers = new Headers({ 'Content-Type': 'application/json' });
    constructor(private http: Http) {
    }

    /*This service used for add new mail setting*
     * /
     * @param consumerAppsAc
     */
    addMailSetting(mailSetting: MailSettingAC) {
        return this.http
            .post(this.mailSettingUrl, JSON.stringify(mailSetting), { headers: this.headers })
            .toPromise();
    }

  /*This service used for get project details by id.*
  *
  * @param id
  */
    getProjectByIdAndModule(id: number, module: string) {
        return this.http.get(this.mailSettingUrl + "/project/" + id + "/" + module)
            .map(res => res.json())
            .toPromise();
    }

    /*This service used for get list of projects.*
    *
    */
    getAllProjects() {
        return this.http.get(this.mailSettingUrl + "/project")
            .map(res => res.json()).toPromise();
    }

    /*This service used for update new mail setting*
    * /
    * @param consumerAppsAc
    */
    updateMailSetting(mailSetting: MailSettingAC) {
        return this.http
            .put(this.mailSettingUrl, JSON.stringify(mailSetting), { headers: this.headers })
            .toPromise();
    }

    /*This service used for getting list of Groups*
    * /
    * @param consumerAppsAc
    */
    getListOfGroups() {
        return this.http.get(this.mailSettingUrl + "/group")
            .map(res => res.json()).toPromise();
    }
}
