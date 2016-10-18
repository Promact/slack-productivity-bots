import { Component, OnInit } from '@angular/core';
import {Router, ActivatedRoute } from '@angular/router';
import { TaskService }   from '../taskmail.service';
import {taskmailuserModel} from '../taskmailuser.model';
import {TaskMailStatus} from '../../enums/TaskMailStatus';
import { LoaderService } from '../../shared/loader.service';
import {StringConstant} from '../../shared/stringConstant';

@Component({
    templateUrl: "app/taskmail/taskmail-list/taskmail-list.html",
    providers: [StringConstant]
})
export class TaskMailListComponent {
    listOfUsers: any;
    constructor(private router: Router, private taskService: TaskService,
        private stringConstant: StringConstant,private loader: LoaderService) {

    }

    ngOnInit() {
        
        this.getListOfEmployee();
        
    }
    getListOfEmployee()
    {
        this.loader.loader = true;
        this.taskService.getListOfEmployee().subscribe((result) => {

            if (result.length > 0)
                if (result[0].UserRole === this.stringConstant.RoleAdmin) {
                    this.listOfUsers = result;
                }
                else if (result[0].UserRole === this.stringConstant.RoleTeamLeader) {
                    var UserId = result[0].UserId;
                    var UserRole = result[0].UserRole;
                    var UserName = result[0].UserName;
                    var UserEmail = result[0].UserEmail;
                    this.router.navigate([this.stringConstant.taskDetails, UserId, UserRole, UserName]);
                }
                else {
                    var UserId = result[0].UserId;
                    var UserRole = result[0].UserRole;
                    var UserName = result[0].UserName;
                    var UserEmail = result[0].UserEmail;
                    this.router.navigate([this.stringConstant.taskDetails, UserId, UserRole, UserName]);
                }
        }, err => {

            });
        this.loader.loader = false;
    }
    taskmailDetails(UserId, UserName, UserEmail)
    {
        var UserRole = this.stringConstant.RoleAdmin;
        this.router.navigate([this.stringConstant.taskDetails, UserId, UserRole, UserName]);//, UserEmail]);
    }
    
}