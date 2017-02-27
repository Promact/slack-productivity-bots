import { Injectable } from '@angular/core';

@Injectable()
export class MockToast {
    show() {
        return true;
    }

    hide() {
        return false;
    }
}