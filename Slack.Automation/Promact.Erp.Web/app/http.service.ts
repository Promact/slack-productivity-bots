import {Http, Headers, RequestOptions, Response} from "@angular/http";
import {Injectable} from "@angular/core";
import {Observable} from 'rxjs/Observable';

import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/Rx';

@Injectable()
export class HttpService<T> {

    constructor(private http: Http) {

    }
    get(url: string) {
        return this.intercept(this.http.get(url).map(res => res.json()));
    }

    post(url: string, body: T) {
        let jsonBody = JSON.stringify(body);
        let headers = new Headers({ 'Content-Type': 'application/json; charset=utf-8' });
        let options = new RequestOptions({ headers: headers });

        return this.intercept(this.http.post(url, jsonBody, options).map(res => res.json()));
    }

    put(url: string, body: T) {
        let jsonBody = JSON.stringify(body);
        let headers = new Headers({ 'Content-Type': 'application/json; charset=utf-8' });
        let options = new RequestOptions({ headers: headers });

        return this.intercept(this.http.put(url, jsonBody, options).map(res => res.json()));
    }

    delete(url: string) {
        return this.intercept(this.http.delete(url).map(res => res.json()));
    }

    intercept(observable: Observable<Response>) {
        return observable.catch((err, source) => {
            if (err.status === 401) {
                location.href = "/Login";
            }

            else if (err.status === 500) {
                return Observable.throw(err);
            }

            else {
                return Observable.throw(err);
            }
        });

    }

}