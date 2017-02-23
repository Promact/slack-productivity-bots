import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { LoaderService } from '../../shared/loader.service';
import { StringConstant } from '../../shared/stringConstant';
import { GroupService } from '../group.service';

@Component({
    templateUrl: './app/Group/GroupList/GroupList.html',
    providers: [StringConstant]
})
export class GroupListComponent implements OnInit {
    constructor(private router: Router, private stringConstant: StringConstant, private loader: LoaderService, private groupService: GroupService) {

    }

    ngOnInit() {

    }
}

