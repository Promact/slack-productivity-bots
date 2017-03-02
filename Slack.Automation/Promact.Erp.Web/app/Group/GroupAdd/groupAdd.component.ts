import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { LoaderService } from '../../shared/loader.service';
import { StringConstant } from '../../shared/stringConstant';
import { GroupService } from '../group.service';
import { GroupModel } from '../group.model';
import { Md2Toast } from 'md2';

@Component({
    templateUrl: './app/Group/GroupAdd/groupAdd.html'
})
export class GroupAddComponent implements OnInit {
    groupModel: GroupModel;
    validPattern: any;
    isExistsGroupName: boolean;
    constructor(private router: Router, private stringConstant: StringConstant, private loader: LoaderService, private groupService: GroupService, private toast: Md2Toast) {
        this.groupModel = new GroupModel();
        this.validPattern = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        this.isExistsGroupName = false;
    }

    ngOnInit() {
    }

    addGroup(groupModel: GroupModel) {
        this.loader.loader = true;
        groupModel.Type = 2;//static group
        if (!this.isExistsGroupName) {
            this.groupService.addGroup(groupModel).then((result) => {
                this.toast.show("Group added successfully.");
                this.backToGroupList();
                this.loader.loader = false;
            }, err => {

            });
        }
    }

    checkGroupName(groupName: string) {
        if (groupName !== undefined && groupName !== "") {
            this.loader.loader = true;
            this.groupService.checkGroupNameIsExists(groupName, 0).then((result) => {
                this.isExistsGroupName = result
                this.loader.loader = false;
            }, err => {

            });
        }
    }

    backToGroupList() {
        this.router.navigate(['/group']);
    }

}