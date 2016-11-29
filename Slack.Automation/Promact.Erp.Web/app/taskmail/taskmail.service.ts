import { Injectable } from '@angular/core';
//import {HttpService} from "../http.service";
//import 'rxjs/add/operator/toPromise';
import { taskmailModel } from './taskmail.model';
import {taskmailuserModel} from './taskmailuser.model';
import {Http, Headers, RequestOptions, Response} from "@angular/http";
import { Observable } from 'rxjs/Rx';
import { StringConstant } from '../shared/stringConstant';

@Injectable()
export class TaskService {
    private TaskMailUrl = 'api/TaskReport';  // URL to web api
    constructor(private http: Http, private stringConstant: StringConstant) { }
    
    getListOfEmployee(): Observable<taskmailuserModel[]> {
        return this.http.get(this.TaskMailUrl)
            .map(this.extractData)
            .catch(this.handleError);
    }
    
    getTaskMailDetailsReport(UserId: string, UserRole: string, UserName: string): Observable<taskmailuserModel[]> {
        return this.http.get(this.TaskMailUrl + this.stringConstant.slash + UserId + this.stringConstant.slash + UserRole + this.stringConstant.slash + UserName)
            .map(this.extractData)
            .catch(this.handleError);
    }

    getTaskMailDetailsReportPreviousDate(UserName: string,UserId: string, UserRole: string, CreatedOns: string): Observable<taskmailuserModel[]> {
        return this.http.get(this.TaskMailUrl +"/taskMailDetailsReportPreviousDate/" + UserRole + "/" + CreatedOns + "/" + UserId + "/" + UserName)
            .map(this.extractData)
            .catch(this.handleError);
    }
    getTaskMailDetailsReportNextDate(UserName: string,UserId: string, UserRole: string, CreatedOns: string): Observable<taskmailuserModel[]> {
        return this.http.get(this.TaskMailUrl +"/taskMailDetailsReportNextDate/" + UserRole + "/" + CreatedOns + "/" + UserId + "/" + UserName)
            .map(this.extractData)
            .catch(this.handleError);
    }

    getTaskMailDetailsReportSelectedDate(UserName: string, UserId: string, UserRole: string, CreatedOns: string, SelectedDate: string): Observable<taskmailuserModel[]> {
        return this.http.get(this.TaskMailUrl + this.stringConstant.slash + UserRole + this.stringConstant.slash + CreatedOns + this.stringConstant.slash + UserId + this.stringConstant.slash + UserName + this.stringConstant.slash + SelectedDate)
            .map(this.extractData)
            .catch(this.handleError);
    }

    private extractData(res: Response) {
        let body = res.json();
        return body || {};
    }

    private handleError(error: any) {
        let errMsg = 'Server error';
        return Observable.throw(errMsg);
    }
    
}
