import { Injectable } from '@angular/core';
import { TaskMailDetailsModel } from './taskmaildetails.model';
import { TaskMailModel } from './taskmail.model';
import { Http, Headers, RequestOptions, Response } from "@angular/http";
import { Observable } from 'rxjs/Rx';
import { StringConstant } from '../shared/stringConstant';
import { URLSearchParams, QueryEncoder } from '@angular/http';

@Injectable()
export class TaskService {
    private TaskMailUrl = this.stringConstant.taskMaiUrl;  // URL to web api
    constructor(private http: Http, private stringConstant: StringConstant) { }

    getListOfEmployee(): Observable<TaskMailModel[]> {
        return this.http.get(this.TaskMailUrl)
            .map(this.extractData)
            .catch(this.handleError);
    }

    getTaskMailDetailsReport(userId: string, role: string, userName: string): Observable<TaskMailModel[]> {
        let params = new URLSearchParams();
        params.set(this.stringConstant.role, role);
        params.set(this.stringConstant.name, userName);
        return this.http.get(this.TaskMailUrl + this.stringConstant.taskDetailsUrl + userId, { search: params })
            .map(this.extractData)
            .catch(this.handleError);
    }

    getTaskMailDetailsReportPreviousDate(userName: string, userId: string, role: string, createdOns: string): Observable<TaskMailModel[]> {
        let params = new URLSearchParams();
        params.set(this.stringConstant.role, role);
        params.set(this.stringConstant.name, userName);
        params.set(this.stringConstant.createdOns, createdOns);
        params.set(this.stringConstant.pageType, this.stringConstant.previous);
        return this.http.get(this.TaskMailUrl + this.stringConstant.taskDetailsUrl + userId, { search: params })
            .map(this.extractData)
            .catch(this.handleError);
    }
    getTaskMailDetailsReportNextDate(userName: string, userId: string, role: string, createdOns: string): Observable<TaskMailModel[]> {
        let params = new URLSearchParams();
        params.set(this.stringConstant.role, role);
        params.set(this.stringConstant.name, userName);
        params.set(this.stringConstant.createdOns, createdOns);
        params.set(this.stringConstant.pageType, this.stringConstant.next);
        return this.http.get(this.TaskMailUrl + this.stringConstant.taskDetailsUrl + userId, { search: params })
            .map(this.extractData)
            .catch(this.handleError);
    }

    getTaskMailDetailsReportSelectedDate(userName: string, userId: string, role: string, createdOns: string, selectedDate: string): Observable<TaskMailModel[]> {
        let params = new URLSearchParams();
        params.set(this.stringConstant.role, role);
        params.set(this.stringConstant.name, userName);
        params.set(this.stringConstant.createdOns, createdOns);
        params.set(this.stringConstant.selectedDate, selectedDate);
        return this.http.get(this.TaskMailUrl + this.stringConstant.taskDetailsUrl + userId, { search: params })
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