import { Component, OnInit } from '@angular/core';
import {ROUTER_DIRECTIVES, Router, ActivatedRoute } from '@angular/router';
import { TaskService }   from '../taskmail.service';
//import {taskmailModel} from '../taskmail.model';
import {taskmailuserModel} from '../taskmailuser.model';
import {TaskMailStatus} from '../../enums/TaskMailStatus';
import { SpinnerService} from '../../spinner.service';
@Component({
    templateUrl: "app/taskmail/taskmail-list/taskmail-list.html",
    directives: [ROUTER_DIRECTIVES],
    
})
export class TaskMailListComponent {
    listOfUsers: any;
    constructor(private router: Router, private taskService: TaskService, private spinner: SpinnerService) {

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
                if (result[0].UserRole == "Admin") {
                    this.listOfUsers = result;
                }
                else if (result[0].UserRole == "TeamLeader") {
                    var UserId = result[0].UserId;
                    var UserRole = result[0].UserRole;
                    var UserName = result[0].UserName;
                    var UserEmail = result[0].UserEmail;
                    this.router.navigate(['task/taskdetail', UserId, UserRole, UserName]);//]);//, UserEmail]);
                }
                else {
                    var UserId = result[0].UserId;
                    var UserRole = result[0].UserRole;
                    var UserName = result[0].UserName;
                    var UserEmail = result[0].UserEmail;
                    this.router.navigate(['task/taskdetail', UserId, UserRole, UserName]);//]);//,UserEmail]);
                }
        }, err => {

        });
    }
    taskmailDetails(UserId, UserName, UserEmail)
    {
        var UserRole = "Admin";
        this.router.navigate(['task/taskdetail', UserId, UserRole, UserName]);//, UserEmail]);
    }
    
}