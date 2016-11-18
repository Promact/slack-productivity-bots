import {Component, OnInit} from "@angular/core";
import {Router, ActivatedRoute } from '@angular/router';
import { TaskService }   from '../taskmail.service';

import { TaskMailDetailsModel } from '../../taskmail/taskmaildetails.model';
import { TaskMailModel } from '../taskmail.model';

import {TaskMailStatus} from '../../enums/TaskMailStatus';
import { DatePipe } from '@angular/common';
import { StringConstant } from '../../shared/stringConstant';
import { LoaderService } from '../../shared/loader.service';

@Component({
    selector: 'date-pipe',
    templateUrl: "app/taskmail/taskmail-details/taskmail-details.html",
    providers: [StringConstant]
})
export class TaskMailDetailsComponent implements OnInit {
    taskMail: Array<TaskMailModel>;
    taskMailDetails: Array<TaskMailDetailsModel>;
    public UserId: string;
    public UserRole: string;
    public UserName: string;
    public isMax: boolean;
    public isMin: boolean;
    public SelectedDate: string;
    public isMaxDate: string;
    public isMinDate: string;
    public isHide: boolean;
    //public UserEmail: string;
    constructor(private route: ActivatedRoute, private router: Router, private taskService: TaskService, private spinner: SpinnerService, private stringConstant: StringConstant) {
        this.taskMailDetails = new Array<TaskMailDetailsModel>();

    }
    ngOnInit() {
        
        this.getTaskMailDetails();
        
    }
    getTaskMailDetails()
    {
        this.loader.loader = true;
        this.route.params.subscribe(params => {
            this.UserId = params['UserId']; 
            this.UserRole = params['UserRole'];
            this.UserName = params['UserName'];
            if (this.UserRole === this.stringConstant.RoleAdmin) {
                this.isHide = false;
            }
            else {
                this.isHide = true;
            }
            this.isMax = true;
            this.taskService.getTaskMailDetailsReport(this.UserId, this.UserRole, this.UserName).subscribe(taskMailUser => {
                this.taskMail = taskMailUser;
                let datePipeMinDate = new DatePipe("medium");
                this.isMinDate = datePipeMinDate.transform(this.taskMail[0].IsMin, this.stringConstant.dateDefaultFormat);
                if (this.isMinDate === this.stringConstant.defaultDate) {
                    this.isMin = true;
                }
                let datePipeMaxDate = new DatePipe("medium");
                this.isMaxDate = datePipeMaxDate.transform(this.taskMail[0].IsMax, this.stringConstant.dateDefaultFormat);
                this.taskMail.forEach(taskmailuser => {
                    let datePipe = new DatePipe("medium");
                    taskmailuser.CreatedOns = datePipe.transform(taskmailuser.CreatedOn, this.stringConstant.dateFormat);
                    taskmailuser.TaskMails.forEach(taskMail => {
                        if (taskMail.Comment === this.stringConstant.notAvailableComment) {
                            taskMail.StatusName = this.stringConstant.notAvailableComment;
                        }
                        else {
                            taskMail.StatusName = TaskMailStatus[taskMail.Status];
                        }
                    });
                });
                this.loader.loader = false;
            });
        });
      
    }
    getTaskMailList() {
        this.router.navigate([this.stringConstant.taskList]);
    }
    getTaskMailPrevious(UserName, UserId, UserRole, CreatedOn) {
        this.SelectedDate = "";
       this.taskService.getTaskMailDetailsReportPreviousDate(UserName,UserId,UserRole, CreatedOn).subscribe(taskMailUser => {
           this.taskMail = taskMailUser;
           if (this.taskMail[0].IsMin === this.taskMail[0].CreatedOn) {
                    this.isMin = true;
                }
           this.taskMail.forEach(taskmailuser => {
                    let datePipe = new DatePipe("medium");
                    taskmailuser.CreatedOns = datePipe.transform(taskmailuser.CreatedOn, this.stringConstant.dateFormat);
                    taskmailuser.TaskMails.forEach(taskMail => {
                        if (taskMail.Comment === this.stringConstant.notAvailableComment) {
                            taskMail.StatusName = this.stringConstant.notAvailableComment;
                        }
                        else {
                            taskMail.StatusName = TaskMailStatus[taskMail.Status];
                        }
                    });
                });
        });
        this.isMax = false;
    }
    getTaskMailNext(UserName, UserId, UserRole, CreatedOn) {
        this.SelectedDate = "";
        this.taskService.getTaskMailDetailsReportNextDate(UserName, UserId, UserRole, CreatedOn).subscribe(taskMailUser => {
            this.taskMail = taskMailUser;
            if (this.taskMail[0].IsMax === this.taskMail[0].CreatedOn) {
                this.isMax = true;
            }
            this.isMin = false;
            this.taskMail.forEach(taskmailuser => {
                let datePipe = new DatePipe("medium");
                taskmailuser.CreatedOns = datePipe.transform(taskmailuser.CreatedOn, this.stringConstant.dateFormat);
                taskmailuser.TaskMails.forEach(taskMail => {
                    if (taskMail.Comment === this.stringConstant.notAvailableComment) {
                        taskMail.StatusName = this.stringConstant.notAvailableComment;
                    }
                    else {
                        taskMail.StatusName = TaskMailStatus[taskMail.Status];
                    }
                });
            });
            
        });
    }
    getTaskMailForSelectedDate(UserName, UserId, UserRole, CreatedOn, SelectedDate) {
        this.taskService.getTaskMailDetailsReportSelectedDate(UserName, UserId, UserRole, CreatedOn, SelectedDate).subscribe(taskMailUser => {
            this.taskMail = taskMailUser;
            if (this.taskMail[0].IsMax === this.taskMail[0].CreatedOn) {
                this.isMax = true;
                this.isMin = false;
            }
            if (this.taskMail[0].IsMin === this.taskMail[0].CreatedOn) {
                this.isMax = false;
                this.isMin = true;
            }
            this.taskMail.forEach(taskmailuser => {
                let datePipe = new DatePipe("medium");
                taskmailuser.CreatedOns = datePipe.transform(taskmailuser.CreatedOn, this.stringConstant.dateFormat);
                taskmailuser.TaskMails.forEach(taskMail => {
                    if (taskMail.Comment === this.stringConstant.notAvailableComment) {
                        taskMail.StatusName = this.stringConstant.notAvailableComment;
                    }
                    else {
                        taskMail.StatusName = TaskMailStatus[taskMail.Status];
                    }
                });
            });

        });
    }
}