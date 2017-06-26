import { Component, OnInit } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { LoaderService } from '../../shared/loader.service';
import { StringConstant } from '../../shared/stringConstant';
import { GroupService } from '../group.service';
import { GroupModel } from '../group.model';
import { Md2Toast } from 'md2';
import { MaterialAutoSelectChip } from '../../shared/angular-material-chip-autoselect.service';

@Component({
    moduleId: module.id,
    templateUrl: 'groupEdit.html',
})
export class GroupEditComponent implements OnInit {
    validPattern: any;
    groupModel: GroupModel;
    isExistsGroupName: boolean;
    id: number;
    listOfActiveEmail: Array<string>;
    emailHasValue: boolean;

    constructor(private router: Router, private route: ActivatedRoute, private stringConstant: StringConstant,
        private loader: LoaderService, private groupService: GroupService, private toast: Md2Toast,
        private materialAutoSelectChipService: MaterialAutoSelectChip) {
        this.validPattern = this.stringConstant.emailValidPattern;
        this.groupModel = new GroupModel();
        this.isExistsGroupName = false;
        this.emailHasValue = false;
    }

    ngOnInit() {
        this.loader.loader = true;
        this.getActiveUserEmailList();
        this.route.params.subscribe(params => {
            this.id = params[this.stringConstant.paramsId];
            this.groupService.getGroupbyId(this.id).then((result) => {
                this.groupModel = result;
                this.loader.loader = false;
            }, err => {
                if (err.status === 400) {
                    this.toast.show('Group not found.');
                }
                this.loader.loader = false;
            });
        });
    }

    getActiveUserEmailList() {
        this.groupService.getActiveUserEmailList().then((result) => {
            this.listOfActiveEmail = result;
        });
    }

    updateGroup(groupModel: GroupModel) {
        this.loader.loader = true;
        if (!this.isExistsGroupName) {
            this.groupService.updateGroup(groupModel).then((result) => {
                this.backToGroupList();
                this.toast.show('Group updated successfully.');
                this.loader.loader = false;
                this.emailHasValue = true;
            }, err => {
                this.toast.show('Group must not be updated.');
                this.loader.loader = false;
            });
        }
    }

    checkGroupName(groupName: string) {
        if (groupName !== undefined && groupName !== "") {
            this.loader.loader = true;
            this.groupService.checkGroupNameIsExists(groupName, this.id).then((result) => {
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