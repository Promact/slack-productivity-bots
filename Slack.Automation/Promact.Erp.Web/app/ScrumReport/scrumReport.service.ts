import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response} from "@angular/http";
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
        return this.http.get(this.stringConstant.scrum)
            .map(res => res.json())
            .catch(this.handleError);
    }

    /*This method is used to get scrumdetails for a particular project on a specific date*
     * 
     * @param Id
     * @param Date
     */
    getScrumDetails(id: number, date: string): Observable<ScrumDetails> {
        return this.http.get(this.stringConstant.scrum + this.stringConstant.slash + id + this.stringConstant.detail, { search: new Date(date).toJSON() })
            .map(res => res.json())
            .catch(this.handleError);       
    }

    /*This method is used to handle errors
    *
    *@param error
    */
    private handleError(error: string) {
        let errMsg = this.stringConstant.serverError;
        return Observable.throw(errMsg);
    }
}