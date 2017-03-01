import { Component, OnInit } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { LoaderService } from '../../shared/loader.service';
import { StringConstant } from '../../shared/stringConstant';
import { GroupService } from '../group.service';
import { GroupModel } from '../group.model';
import { Md2Toast } from 'md2';

@Component({
    templateUrl: './app/Group/GroupEdit/groupEdit.html',
})
export class GroupEditComponent implements OnInit {
    validPattern: any;
    groupModel: GroupModel;
    isExistsGroupName: boolean;
    id: number;
    constructor(private router: Router, private route: ActivatedRoute, private stringConstant: StringConstant, private loader: LoaderService, private groupService: GroupService, private toast: Md2Toast) {
        this.validPattern = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        this.groupModel = new GroupModel();
        this.isExistsGroupName = false;
    }

    ngOnInit() {
        this.loader.loader = true;
        this.route.params.subscribe(params => {
            this.id = +this.route.snapshot.params[this.stringConstant.paramsId];
            this.groupService.getGroupbyId(this.id).then((result) => {
                this.groupModel = result
                this.loader.loader = false;
            }, err => {
            });
        });
    }

    updateGroup(groupModel: GroupModel) {
        this.loader.loader = true;
        if (!this.isExistsGroupName) {
            this.groupService.updateGroup(groupModel).then((result) => {
                this.backToGroupList();
                this.toast.show('Group updated successfully. ');
                this.loader.loader = false;

            }, err => {
            });
        }
    }

    checkGroupName(groupName: string) {
        if (groupName !== undefined && groupName !== "") {
            this.loader.loader = true;
            this.groupService.checkGroupNameIsExists(groupName, this.id).then((result) => {
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