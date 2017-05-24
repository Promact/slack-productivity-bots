import { Injectable, OnInit } from '@angular/core';

@Injectable()
export class MaterialAutoSelectChip {
    constructor() { }

    selectGroup(group: string, list: Array<string>) {
        let index = list.indexOf(group);
        if (index === -1) {
            list.push(group);
        }
        return list;
    }

    removeGroup(group: string, list: Array<string>) {
        let index = list.indexOf(group);
        if (index !== -1) {
            list.splice(list.indexOf(group), 1);
        }
        return list;
    }
}