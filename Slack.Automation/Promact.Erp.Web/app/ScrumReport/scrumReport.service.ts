import { Injectable } from '@angular/core';
import {Http, Headers, RequestOptions, Response} from "@angular/http";

import { Observable } from 'rxjs/Rx';
import { ScrumProject } from './scrumProject-List/scrumProject-List.model';
import { ScrumDetails } from './scrumProject-Details/scrumProject-Details.model';
import { StringConstant } from '../shared/stringConstant';

@Injectable()

export class ScrumReportService {
   
    constructor(private http: Http, private stringConstant: StringConstant) { }

     /*This method is used to get list of projects*
     * 
     */
    getScrumProjects(): Observable<ScrumProject[]> {
        return this.http.get(this.stringConstant.scrumProjects)
            .map(res => res.json())
            .catch(this.handleError);
    }

    /*This method is used to get scrumdetails for a particular project on a specific date*
     * 
     * @param Id
     * @param Date
     */
    getScrumDetails(Id: number, Date: any): Observable<ScrumDetails> {
        return this.http.get((this.stringConstant.scrumDetails + this.stringConstant.slash + Id + this.stringConstant.slash + Date))
            .map(res => res.json())
            .catch(this.handleError);       
    }

    /*This method is used to handle errors
    *
    */
    private handleError(error: any) {
        let errMsg = this.stringConstant.serverError;
        return Observable.throw(errMsg);
    }
}