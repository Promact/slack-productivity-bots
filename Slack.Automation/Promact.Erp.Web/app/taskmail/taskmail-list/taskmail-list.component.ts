import { Component, OnInit } from '@angular/core';
import {Router } from '@angular/router';
import { TaskService }   from '../taskmail.service';
import { TaskMailModel } from '../taskmail.model';
import { TaskMailStatus } from '../../enums/TaskMailStatus';
import { LoaderService } from '../../shared/loader.service';
import {StringConstant} from '../../shared/stringConstant';

@Component({
    moduleId: module.id,
    templateUrl: "taskmail-list.html",
    providers: [StringConstant]
})
export class TaskMailListComponent implements OnInit {
    taskMailUsers: Array<TaskMailModel>;
    constructor(private router: Router, private taskService: TaskService, private stringConstant: StringConstant, private loader: LoaderService) {

    }

    ngOnInit() {
        
        this.getListOfEmployee();
        
    }
    getListOfEmployee() {
        this.loader.loader = true;
        this.taskService.getListOfEmployee().subscribe((result) => {

            if (result.length > 0)
                if (result[0].UserRole === this.stringConstant.RoleAdmin) {
                    this.taskMailUsers = result;
                }
                else if (result[0].UserRole === this.stringConstant.RoleTeamLeader) {
                    this.router.navigate([this.stringConstant.taskDetails, result[0].UserId, result[0].UserRole, result[0].UserName]);
                }
                else {
                    this.router.navigate([this.stringConstant.taskDetails, result[0].UserId, result[0].UserRole, result[0].UserName]);
                }
            this.loader.loader = false;
        }, err => {

            });
       
    }
    taskmailDetails(UserId: any, UserName: any, UserEmail: any) {
        let UserRole = this.stringConstant.RoleAdmin;
        this.router.navigate([this.stringConstant.taskDetails, UserId, UserRole, UserName]);
    }
    
}