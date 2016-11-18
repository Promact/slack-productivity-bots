import { Injectable } from '@angular/core';
//import {HttpService} from "../http.service";
//import 'rxjs/add/operator/toPromise';
import { taskmailModel } from './taskmail.model';
import {taskmailuserModel} from './taskmailuser.model';
import {Http, Headers, RequestOptions, Response} from "@angular/http";
import { Observable } from 'rxjs/Rx';

@Injectable()
export class TaskService {
    private TaskMailUrl = 'api/TaskReport';  // URL to web api
    constructor(private http: Http) { }
    
    getListOfEmployee(): Observable<taskmailuserModel[]> {
        return this.http.get("getAllEmployee/")
            .map(this.extractData)
            .catch(this.handleError);
    }
    getTaskMailReport(currentPage: number,itemsPerPage:number): Observable<taskmailModel[]> {
        //return this.httpService.get("taskMailReport");
        return this.http.get("taskMailReport/" + currentPage + "/" + itemsPerPage)
            .map(this.extractData)
            .catch(this.handleError);
    }
    getTaskMailDetailsReport(UserId: string, UserRole: string, UserName: string): Observable<taskmailuserModel[]> {//, UserName: string, UserEmail: string): Observable<taskmailuserModel[]> {
        //return this.httpService.get("taskMailDetailsReport/"+ id);
        return this.http.get("taskMailDetailsReport/" + UserId + "/" + UserRole + "/" + UserName)
            .map(this.extractData)
            .catch(this.handleError);
    }


    getTaskMailDetailsReportPreviousDate(UserName: string,UserId: string, UserRole: string, CreatedOns: string): Observable<taskmailuserModel[]> {
        return this.http.get("taskMailDetailsReportPreviousDate/" + UserRole + "/" + CreatedOns + "/" + UserId + "/" + UserName)
            .map(this.extractData)
            .catch(this.handleError);
    }
    getTaskMailDetailsReportNextDate(UserName: string,UserId: string, UserRole: string, CreatedOns: string): Observable<taskmailuserModel[]> {
        return this.http.get("taskMailDetailsReportNextDate/" + UserRole + "/" + CreatedOns + "/" + UserId + "/" + UserName)
            .map(this.extractData)
            .catch(this.handleError);
    }

    getTaskMailDetailsReportSelectedDate(UserName: string, UserId: string, UserRole: string, CreatedOns: string, SelectedDate: string): Observable<taskmailuserModel[]> {
        return this.http.get("taskMailDetailsReportSelectedDate/" + UserRole + "/" + CreatedOns + "/" + UserId + "/" + UserName + "/" + SelectedDate)
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
