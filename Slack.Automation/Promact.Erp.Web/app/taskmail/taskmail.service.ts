import { Injectable } from '@angular/core';
//import {HttpService} from "../http.service";
//import 'rxjs/add/operator/toPromise';
import { taskmailModel } from './taskmail.model';
import {Http, Headers, RequestOptions, Response} from "@angular/http";
import { Observable } from 'rxjs/Rx';

@Injectable()
export class TaskService {
    private TaskMailUrl = 'api/TaskReport';  // URL to web api
    constructor(private http: Http) { }
    
    //getTaskMailReport():Observable<taskmailModel[]> {
    //    //return this.httpService.get("taskMailReport");
    //    return this.http.get("taskMailReport")
    //        .map(this.extractData)
    //        .catch(this.handleError);
    //}
    getTaskMailReport(currentPage: number,itemsPerPage:number): Observable<taskmailModel[]> {
        //return this.httpService.get("taskMailReport");
        return this.http.get("taskMailReport/" + currentPage + "/" + itemsPerPage)
            .map(this.extractData)
            .catch(this.handleError);
    }

    getTaskMailDetailsReport(id: number): Observable<taskmailModel[]> {
        //return this.httpService.get("taskMailDetailsReport/"+ id);
        return this.http.get("taskMailDetailsReport/" + id)
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
