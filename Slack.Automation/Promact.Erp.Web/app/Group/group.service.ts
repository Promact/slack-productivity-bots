import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from "@angular/http";
import { Observable } from 'rxjs/Rx';
import { StringConstant } from '../shared/stringConstant';

@Injectable()
export class GroupService {
    constructor(private http: Http, private stringConstant: StringConstant) {



    }
}

