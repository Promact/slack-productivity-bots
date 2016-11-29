import { Injectable } from '@angular/core';
import { taskmailModel } from './taskmail.model';
import {taskmailuserModel} from './taskmailuser.model';
import {Http, Headers, RequestOptions, Response} from "@angular/http";
import { Observable } from 'rxjs/Rx';
import { StringConstant } from '../shared/stringConstant';
import { URLSearchParams, QueryEncoder } from '@angular/http';

@Injectable()
export class TaskService {
    private TaskMailUrl = this.stringConstant.taskMaiUrl;  // URL to web api
    constructor(private http: Http, private stringConstant: StringConstant) { }
    
    getListOfEmployee(): Observable<taskmailuserModel[]> {
        return this.http.get(this.TaskMailUrl)
            .map(this.extractData)
            .catch(this.handleError);
    }
    
    getTaskMailDetailsReport(UserId: string, UserRole: string, UserName: string): Observable<taskmailuserModel[]> {
        let params = new URLSearchParams();
        params.set(this.stringConstant.userRole, UserRole);
        params.set(this.stringConstant.userName, UserName);
        return this.http.get(this.TaskMailUrl+ this.stringConstant.slash + UserId, { search: params })
            .map(this.extractData)
            .catch(this.handleError);
    }

    getTaskMailDetailsReportPreviousDate(UserName: string,UserId: string, UserRole: string, CreatedOns: string): Observable<taskmailuserModel[]> {
        let params = new URLSearchParams();
        params.set(this.stringConstant.userRole, UserRole);
        params.set(this.stringConstant.userName, UserName);
        params.set(this.stringConstant.createdOns, CreatedOns);
        return this.http.get(this.TaskMailUrl + this.stringConstant.slash + UserId + this.stringConstant.taskDetailsUrl, { search: params })
            .map(this.extractData)
            .catch(this.handleError);
    }
    getTaskMailDetailsReportNextDate(UserName: string, UserId: string, UserRole: string, CreatedOns: string): Observable<taskmailuserModel[]> {
        let params = new URLSearchParams();
        params.set(this.stringConstant.userRole, UserRole);
        params.set(this.stringConstant.userName, UserName);
        params.set(this.stringConstant.createdOns, CreatedOns);
        return this.http.get(this.TaskMailUrl + this.stringConstant.slash + this.stringConstant.taskDetailsUrl + this.stringConstant.slash + UserId, { search: params })
            .map(this.extractData)
            .catch(this.handleError); 
    }

    getTaskMailDetailsReportSelectedDate(UserName: string, UserId: string, UserRole: string, CreatedOns: string, SelectedDate: string): Observable<taskmailuserModel[]> {
        let params = new URLSearchParams();
        params.set(this.stringConstant.userRole, UserRole);
        params.set(this.stringConstant.userName, UserName);
        params.set(this.stringConstant.createdOns, CreatedOns);
        params.set(this.stringConstant.selectedDate, SelectedDate);
        return this.http.get(this.TaskMailUrl + this.stringConstant.slash + UserId, { search: params })
            .map(this.extractData)
            .catch(this.handleError);
    }

    private extractData(res: Response) {
        let body = res.json();
        return body || {};
    }

    private handleError(error: Observable<Response>) {
        let errMsg = 'Server error';
        return Observable.throw(errMsg);
    }
    
}
