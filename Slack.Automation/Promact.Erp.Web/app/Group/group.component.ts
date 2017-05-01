import { Component } from "@angular/core";
import { Router } from "@angular/router";
import { GroupService } from "./group.service";

@Component
    ({
        moduleId: module.id,
        template: `<router-outlet></router-outlet>`,
        providers: [GroupService]
    })
export class GroupComponent { }
