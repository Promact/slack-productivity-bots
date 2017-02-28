import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { LoaderService } from '../../shared/loader.service';
import { StringConstant } from '../../shared/stringConstant';
import { GroupService } from '../group.service';
import { GroupModel } from '../group.model';

@Component({
    templateUrl: './app/Group/GroupList/GroupList.html',
    providers: [StringConstant]
})
export class GroupListComponent implements OnInit {
    groupList: Array<GroupModel>;
    groupId: number;
    constructor(private router: Router, private stringConstant: StringConstant, private loader: LoaderService, private groupService: GroupService) {

    }

    ngOnInit() {
        this.loader.loader = true;
        this.groupService.getListOfGroup().then((result) => {
            this.groupList = result;
            this.loader.loader = false;
        }, err => {

        });
    }

    addNewGroup() {
        this.router.navigate(['/group/add']);
    }

    editGroup(id: number) {
        this.router.navigate(['/group/edit', id]);
    }

    delteGroupPopup(id: number) {
    }

    deleteGroup() { }
}

