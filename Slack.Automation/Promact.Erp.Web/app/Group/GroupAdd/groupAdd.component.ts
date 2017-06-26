import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { LoaderService } from '../../shared/loader.service';
import { StringConstant } from '../../shared/stringConstant';
import { GroupService } from '../group.service';
import { GroupModel } from '../group.model';
import { Md2Toast } from 'md2';
import { MaterialAutoSelectChip } from '../../shared/angular-material-chip-autoselect.service';

@Component({
    moduleId : module.id,
    templateUrl: 'groupAdd.html'
})
export class GroupAddComponent implements OnInit {
    groupModel: GroupModel;
    validPattern: any;
    isExistsGroupName: boolean;
    listOfActiveEmail: Array<string>;
    emailHasValue: boolean;

    constructor(private router: Router, private stringConstant: StringConstant, private loader: LoaderService,
        private groupService: GroupService, private toast: Md2Toast, private materialAutoSelectChipService: MaterialAutoSelectChip) {
        this.groupModel = new GroupModel();
        this.validPattern = this.stringConstant.emailValidPattern;
        this.isExistsGroupName = false;
        this.emailHasValue = false;
        this.groupModel.Emails = new Array<string>();
    }

    ngOnInit() {
        this.loader.loader = true;
        this.groupService.getActiveUserEmailList().then((result) => {
            this.listOfActiveEmail = result;
            this.loader.loader = false;
        });
        this.emailHasValue = false;
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
                this.isExistsGroupName = result;
                this.loader.loader = false;
            }, err => {

            });
        }
    }

    backToGroupList() {
        this.router.navigate(['/group']);
    }

    selectEmail(email: string) {
        this.groupModel.Emails = this.materialAutoSelectChipService.selectGroup(email, this.groupModel.Emails);
        this.emailHasValue = true;
    }

    removeEmail(email: string) {
        this.groupModel.Emails = this.materialAutoSelectChipService.removeGroup(email, this.groupModel.Emails);
        if (this.groupModel.Emails.length === 0) {
            this.emailHasValue = false;
        }
        else {
            this.emailHasValue = true;
        }
    }
}