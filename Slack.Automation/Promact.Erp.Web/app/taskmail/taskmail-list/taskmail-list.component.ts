import { Component, OnInit } from '@angular/core';
import {Router, ActivatedRoute } from '@angular/router';
import { TaskService }   from '../taskmail.service';

import { TaskMailModel } from '../taskmail.model';

import { TaskMailStatus } from '../../enums/TaskMailStatus';
import { LoaderService } from '../../shared/loader.service';
import {StringConstant} from '../../shared/stringConstant';

@Component({
    templateUrl: "app/taskmail/taskmail-list/taskmail-list.html",
    providers: [StringConstant]
})
export class TaskMailListComponent {
    listOfUsers: any;
    constructor(private router: Router, private taskService: TaskService, private loader: LoaderService, private stringConstant: StringConstant) {

    }

    ngOnInit() {
        
        this.getListOfEmployee();
        
    }
    getListOfEmployee() {

        this.taskService.getListOfEmployee().subscribe((result) => {

            if (result.length > 0)
                if (result[0].UserRole === this.stringConstant.RoleAdmin) {
                    this.taskMailUsers = result;
                }
                else if (result[0].UserRole === this.stringConstant.RoleTeamLeader) {
                    let UserId = result[0].UserId;
                    let UserRole = result[0].UserRole;
                    let UserName = result[0].UserName;
                    let UserEmail = result[0].UserEmail;
                    this.router.navigate([this.stringConstant.taskDetails, UserId, UserRole, UserName]);//]);//, UserEmail]);
                }
                else {
                    let UserId = result[0].UserId;
                    let UserRole = result[0].UserRole;
                    let UserName = result[0].UserName;
                    let UserEmail = result[0].UserEmail;
                    this.router.navigate([this.stringConstant.taskDetails, UserId, UserRole, UserName]);//]);//,UserEmail]);
                }
            this.loader.loader = false;
        }, err => {

            });
       
    }
    taskmailDetails(UserId, UserName, UserEmail) {
        let UserRole = this.stringConstant.RoleAdmin;
        this.router.navigate([this.stringConstant.taskDetails, UserId, UserRole, UserName]);//, UserEmail]);
    }
    
}