import { Injectable } from '@angular/core';
import {Http, Headers, RequestOptions, Response} from "@angular/http";

import { Observable } from 'rxjs/Rx';
import { ScrumProject } from './scrumProject-List/scrumProject-List.model';
import { ScrumDetails } from './scrumProject-Details/scrumProject-Details.model';


@Injectable()

export class ScrumReportService {

    constructor(private http: Http) { }

    getScrumProjects(): Observable<ScrumProject[]> {
        return this.http.get("scrumReport")
            .map(res => res.json())
            .catch(this.handleError);
    }

    getScrumDetails(Id: number, Date: any): Observable<ScrumDetails[]> {
        return this.http.get("scrumDetails/"+ Id + "/" + Date)
            .map(res => res.json())
            .catch(this.handleError);       
    }

    private handleError(error: any) {
        let errMsg = 'Server error';
        return Observable.throw(errMsg);
    }
}