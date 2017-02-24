import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';

@Injectable()

export class MockAppComponentService {

    getUserIsAdminOrNot() {
        return new BehaviorSubject("true").asObservable();
    }
}