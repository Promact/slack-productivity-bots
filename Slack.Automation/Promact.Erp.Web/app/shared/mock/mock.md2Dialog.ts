import { Injectable } from '@angular/core';

@Injectable()
export class MockDialog {
    open() {
        return true;
    }

    close() {
        return false;
    }
}