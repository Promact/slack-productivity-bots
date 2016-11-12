import { Injectable } from '@angular/core';
import {Http, Headers, RequestOptions, Response} from "@angular/http";

import { LeaveReport } from './leaveReport-List/leaveReport-List.model';
import { LeaveReportDetail } from './leaveReport-Details/leaveReport-Details.model';

import { Observable } from 'rxjs/Rx';
import { StringConstant } from '../shared/stringConstant';

@Injectable()

export class LeaveReportService {
    empty: any;
    constructor(private http: Http, private stringConstant: StringConstant) { }

    getLeaveReports(): Observable<LeaveReport[]> {
        return this.http.get(this.stringConstant.leaveReport)
            .map(res => res.json() || {})
            .catch(this.handleError);
    }

    getLeaveReportDetail(id: string): Observable<LeaveReportDetail[]> {
        return this.http.get(this.stringConstant.leaveReport + this.stringConstant.slash + id)
            .map(res => res.json() || {})
            .catch(this.handleError);
    }

    private handleError(error: any) {
        let errMsg = this.stringConstant.serverError;
        return Observable.throw(errMsg);
    }
}
