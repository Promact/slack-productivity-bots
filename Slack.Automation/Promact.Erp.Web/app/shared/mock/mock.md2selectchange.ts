import { Injectable } from '@angular/core';
import { Md2SelectChange } from "md2";

//mock MD2Dialog service
@Injectable()
export class MockSelectChange {
    constructor() { }
    public value: number;
}