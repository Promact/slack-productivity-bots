﻿import { Injectable } from '@angular/core';
import {Http, Headers, RequestOptions, Response} from "@angular/http";

import { Observable } from 'rxjs/Rx';
import { ScrumProject } from './scrumProject-List/scrumProject-List.model';
import { ScrumDetails } from './scrumProject-Details/scrumProject-Details.model';
import { StringConstant } from '../shared/stringConstant';

@Injectable()

export class ScrumReportService {
   
    constructor(private http: Http, private stringConstant: StringConstant) { }

    getScrumProjects(): Observable<ScrumProject[]> {
        return this.http.get(this.stringConstant.scrumReport)
            .map(res => res.json())
            .catch(this.handleError);
    }

    getScrumDetails(Id: number, Date: any): Observable<ScrumDetails> {
        return this.http.get(this.stringConstant.scrumDetails + Id + this.stringConstant.slash + Date)
            .map(res => res.json())
            .catch(this.handleError);       
    }

    private handleError(error: any) {
        let errMsg = this.stringConstant.serverError;
        return Observable.throw(errMsg);
    }
}