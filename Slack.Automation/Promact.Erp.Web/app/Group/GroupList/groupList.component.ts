import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { LoaderService } from '../../shared/loader.service';
import { StringConstant } from '../../shared/stringConstant';
import { GroupService } from '../group.service';
import { GroupModel } from '../group.model';
import { Md2Toast } from 'md2';


@Component({
    moduleId: module.id,
    templateUrl: 'groupList.html'
})

export class GroupListComponent implements OnInit {
    groupList: Array<GroupModel>;
    groupId: number;
    constructor(private router: Router, private stringConstant: StringConstant, private loader: LoaderService, private groupService: GroupService, private toast: Md2Toast) {

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

    delteGroupPopup(id: number, popup: any) {
        this.groupId = id;
        popup.open();
    }

    deleteGroup(popup: any) {
        this.loader.loader = true;
        this.groupService.deleteGroupById(this.groupId).then((result) => {
            this.toast.show("Group deleted sucessfully.");
            this.ngOnInit();
            this.loader.loader = false;
        }, err => {
            this.toast.show("Group must not be deleted.");
            this.loader.loader = false;
        });
        popup.close();
    }

    closeDeletePopup(popup: any) {
        popup.close();
    }
}

