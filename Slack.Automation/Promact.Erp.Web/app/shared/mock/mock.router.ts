import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Rx';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';


export class UrlTree {
    root: any;
    queryParams: {
        [key: string]: string;
    };
    fragment: string;
}

//mock Router service
@Injectable()
export class MockRouter {
    navigate() {
        return true;
    }
    createUrlTree() {
        return new UrlTree();
    }

    serializeUrl() {
        return "test";
    }

    private subject = new BehaviorSubject(this.testParams);
    events = this.subject.asObservable();

    // ActivatedRoute.params is Observable
    // Test parameters
    private _testParams: {};
    get testParams() { return this._testParams; }
    set testParams(params: {}) {
        this._testParams = params;
        this.subject.next(params);
    }
    routerState = {};
    navigateByUrl() {
        return new BehaviorSubject(true).asObservable();
    }
}