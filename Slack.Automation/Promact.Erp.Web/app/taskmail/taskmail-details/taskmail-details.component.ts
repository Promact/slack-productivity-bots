import {Component, OnInit} from "@angular/core";
import {Router, ActivatedRoute } from '@angular/router';
import { TaskService }   from '../taskmail.service';
import { TaskMailDetailsModel } from '../taskmaildetails.model';
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
    public IsMaxDate: boolean;
    public IsMinDate: boolean;
    public SelectedDate: string;
    public MaxDate: string;
    public MinDate: string;
    public IsHide: boolean;
    constructor(private route: ActivatedRoute, private router: Router, private taskService: TaskService, private stringConstant: StringConstant, private loader: LoaderService) {
        this.taskMailDetails = new Array<TaskMailDetailsModel>();

    }
    ngOnInit() {
        
        this.getTaskMailDetails();
        
    }
    getTaskMailDetails() {
        this.loader.loader = true;
        this.route.params.subscribe(params => {
            if (params['UserRole'] === this.stringConstant.RoleAdmin) {
                this.IsHide = false;
            }
            else {
                this.IsHide = true;
            }
            this.IsMaxDate = true;
            this.taskService.getTaskMailDetailsReport(params['UserId'], params['UserRole'], params['UserName']).subscribe(taskMailUser => {
                this.taskMail = taskMailUser;
                let datePipeMinDate = new DatePipe("medium");
                this.MinDate = datePipeMinDate.transform(this.taskMail[0].IsMin, this.stringConstant.dateDefaultFormat);
                if (this.MinDate === this.stringConstant.defaultDate) {
                    this.IsMinDate = true;
                }
                let datePipeMaxDate = new DatePipe("medium");
                this.MaxDate = datePipeMaxDate.transform(this.taskMail[0].IsMax, this.stringConstant.dateDefaultFormat);
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
               this.IsMinDate = true;
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
       this.IsMaxDate = false;
    }
    getTaskMailNext(UserName, UserId, UserRole, CreatedOn) {
        this.SelectedDate = "";
        this.taskService.getTaskMailDetailsReportNextDate(UserName, UserId, UserRole, CreatedOn).subscribe(taskMailUser => {
            this.taskMail = taskMailUser;
            if (this.taskMail[0].IsMax === this.taskMail[0].CreatedOn) {
                this.IsMaxDate = true;
            }
            this.IsMinDate = false;
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
                this.IsMaxDate = true;
                this.IsMinDate = false;
            }
            if (this.taskMail[0].IsMin === this.taskMail[0].CreatedOn) {
                this.IsMaxDate = false;
                this.IsMinDate = true;
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