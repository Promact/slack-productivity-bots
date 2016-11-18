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
    
    constructor(private route: ActivatedRoute, private router: Router, private taskService: TaskService,
        private stringConstant: StringConstant,private loader: LoaderService) {
        this.taskMails = new Array<taskmailModel>();

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
            if (this.UserRole === this.stringConstant.RoleAdmin)
            { this.isHide = false; } else { this.isHide = true; }
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
           this.taskMailUser = taskMailUser;
                if (this.taskMailUser[0].IsMin === this.taskMailUser[0].CreatedOn) {
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
            this.taskMailUser = taskMailUser;
            if (this.taskMailUser[0].IsMax === this.taskMailUser[0].CreatedOn) {
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
            this.taskMailUser = taskMailUser;
            if (this.taskMailUser[0].IsMax === this.taskMailUser[0].CreatedOn) {
                this.isMax = true;
                this.isMin = false;
            }
            if (this.taskMailUser[0].IsMin === this.taskMailUser[0].CreatedOn) {
                this.isMax = false;
                this.isMin = true;
            }
            this.taskMail.forEach(taskmailuser => {
                let datePipe = new DatePipe("medium");
                taskmailuser.CreatedOns = datePipe.transform(taskmailuser.CreatedOn, this.stringConstant.dateFormat);
                taskmailuser.TaskMails.forEach(taskMail => {
                    if (taskMail.Comment === this.stringConstant.notAvailableComment)
                    { taskMail.StatusName = this.stringConstant.notAvailableComment }
                    else {
                        taskMail.StatusName = TaskMailStatus[taskMail.Status];
                    }
                });
            });

        });
    }
}