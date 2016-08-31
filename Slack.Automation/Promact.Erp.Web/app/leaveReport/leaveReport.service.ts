import { Injectable } from '@angular/core';
import {Http, Headers, RequestOptions, Response} from "@angular/http";

import { LeaveReport } from './leaveReport-List/leaveReport-List.model';
import { LeaveReportDetail } from './leaveReport-Details/leaveReport-Details.model';

import { Observable } from 'rxjs/Rx';


@Injectable()

export class LeaveReportService {
    empty: any;
    constructor(private http: Http) { }

    getLeaveReports(): Observable<LeaveReport[]> {
        return this.http.get("leaveReport")
            .map(this.extractData)
            .catch(this.handleError);
    }

    getLeaveReportDetail(Id: string): Observable<LeaveReportDetail[]> {
        return this.http.get("leaveReportDetails/" + Id)
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
