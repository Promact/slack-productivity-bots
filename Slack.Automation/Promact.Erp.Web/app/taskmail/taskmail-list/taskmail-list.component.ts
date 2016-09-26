import { Component, OnInit } from '@angular/core';
import {ROUTER_DIRECTIVES, Router, ActivatedRoute } from '@angular/router';
import { TaskService }   from '../taskmail.service';
//import {taskmailModel} from '../taskmail.model';
import {taskmailuserModel} from '../taskmailuser.model';
import {TaskMailStatus} from '../../enums/TaskMailStatus';
import { SpinnerService} from '../../shared/spinner.service';
import {StringConstant} from '../../shared/stringConstant';

@Component({
    templateUrl: "app/taskmail/taskmail-list/taskmail-list.html",
    directives: [ROUTER_DIRECTIVES],
    providers: [StringConstant]
})
export class TaskMailListComponent {
    listOfUsers: any;
    constructor(private router: Router, private taskService: TaskService, private spinner: SpinnerService, private stringConstant: StringConstant) {

    }

    ngOnInit() {
        this.spinner.start();
        this.getListOfEmployee();
        this.spinner.stop();
    }
    getListOfEmployee()
    {

        this.taskService.getListOfEmployee().subscribe((result) => {

            if (result.length > 0)
                if (result[0].UserRole == this.stringConstant.RoleAdmin) {
                    this.listOfUsers = result;
                }
                else if (result[0].UserRole == this.stringConstant.RoleTeamLeader) {
                    var UserId = result[0].UserId;
                    var UserRole = result[0].UserRole;
                    var UserName = result[0].UserName;
                    var UserEmail = result[0].UserEmail;
                    this.router.navigate([this.stringConstant.taskDetails, UserId, UserRole, UserName]);//]);//, UserEmail]);
                }
                else {
                    var UserId = result[0].UserId;
                    var UserRole = result[0].UserRole;
                    var UserName = result[0].UserName;
                    var UserEmail = result[0].UserEmail;
                    this.router.navigate([this.stringConstant.taskDetails, UserId, UserRole, UserName]);//]);//,UserEmail]);
                }
        }, err => {

        });
    }
    taskmailDetails(UserId, UserName, UserEmail)
    {
        var UserRole = this.stringConstant.RoleAdmin;
        this.router.navigate([this.stringConstant.taskDetails, UserId, UserRole, UserName]);//, UserEmail]);
    }
    
}