import {Component, OnInit} from "@angular/core";
import {ROUTER_DIRECTIVES, Router, ActivatedRoute } from '@angular/router';
import { TaskService }   from '../taskmail.service';
import {taskmailModel} from '../taskmail.model';
import {taskmailuserModel} from '../taskmailuser.model';
import {TaskMailStatus} from '../../enums/TaskMailStatus';
import { DatePipe } from '@angular/common';
import { SpinnerService} from '../../shared/spinner.service';
import {StringConstant} from '../../shared/stringConstant';

@Component({
    templateUrl: "app/taskmail/taskmail-details/taskmail-details.html",
    directives: [ROUTER_DIRECTIVES],
    providers: [StringConstant]
})
export class TaskMailDetailsComponent {
    taskMailUser: Array<taskmailuserModel>;
    taskMails: Array<taskmailModel>;
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
        this.taskMails = new Array<taskmailModel>();
    }
    ngOnInit() {
        this.spinner.start();
        //this.taskService.getListOfEmployee().subscribe((result) => {
        //    if (result.length > 0) {
        //        this.UserId = result[0].UserId;
        //        this.UserRole = result[0].UserRole;
        //        this.UserName = result[0].UserName;
        //        this.getTaskMailDetails();
        //    }
        //}, err => {

        this.getTaskMailDetails();
        //});
        
        this.spinner.stop();
    }
    getTaskMailDetails()
    {
        this.route.params.subscribe(params => {
            this.UserId = params['UserId']; 
            this.UserRole = params['UserRole'];
            this.UserName = params['UserName'];
            if (this.UserRole == this.stringConstant.RoleAdmin)
            { this.isHide = false; } else { this.isHide = true; }
            this.isMax = true;
            this.taskService.getTaskMailDetailsReport(this.UserId, this.UserRole, this.UserName).subscribe(taskMailUser => {
                this.taskMailUser = taskMailUser;
                var datePipeMinDate = new DatePipe();
                this.isMinDate = datePipeMinDate.transform(this.taskMailUser[0].IsMin, this.stringConstant.dateDefaultFormat);
                if (this.isMinDate == this.stringConstant.defaultDate)
                { this.isMin = true; }
                var datePipeMaxDate = new DatePipe();
                this.isMaxDate = datePipeMaxDate.transform(this.taskMailUser[0].IsMax, this.stringConstant.dateDefaultFormat);
                this.taskMailUser.forEach(taskmailuser => {
                    var datePipe = new DatePipe();
                    taskmailuser.CreatedOns = datePipe.transform(taskmailuser.CreatedOn, this.stringConstant.dateFormat);
                    taskmailuser.TaskMails.forEach(taskMail => {
                        if (taskMail.Comment == this.stringConstant.notAvailableComment)
                        { taskMail.StatusName = this.stringConstant.notAvailableComment }
                        else {
                            taskMail.StatusName = TaskMailStatus[taskMail.Status];
                        }
                    });
                });
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
                if (this.taskMailUser[0].IsMin == this.taskMailUser[0].CreatedOn) {
                    this.isMin = true;
                }
                this.taskMailUser.forEach(taskmailuser => {
                    var datePipe = new DatePipe();
                    taskmailuser.CreatedOns = datePipe.transform(taskmailuser.CreatedOn, this.stringConstant.dateFormat);
                    taskmailuser.TaskMails.forEach(taskMail => {
                        if (taskMail.Comment == this.stringConstant.notAvailableComment)
                        { taskMail.StatusName = this.stringConstant.notAvailableComment }
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
            if (this.taskMailUser[0].IsMax == this.taskMailUser[0].CreatedOn) {
                this.isMax = true;
            }
            this.isMin = false;
            this.taskMailUser.forEach(taskmailuser => {
                var datePipe = new DatePipe();
                taskmailuser.CreatedOns = datePipe.transform(taskmailuser.CreatedOn, this.stringConstant.dateFormat);
                taskmailuser.TaskMails.forEach(taskMail => {
                    if (taskMail.Comment == this.stringConstant.notAvailableComment)
                    { taskMail.StatusName = this.stringConstant.notAvailableComment}
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
            if (this.taskMailUser[0].IsMax == this.taskMailUser[0].CreatedOn) {
                this.isMax = true;
                this.isMin = false;
            }
            if (this.taskMailUser[0].IsMin == this.taskMailUser[0].CreatedOn) {
                this.isMax = false;
                this.isMin = true;
            }
            this.taskMailUser.forEach(taskmailuser => {
                var datePipe = new DatePipe();
                taskmailuser.CreatedOns = datePipe.transform(taskmailuser.CreatedOn, this.stringConstant.dateFormat);
                taskmailuser.TaskMails.forEach(taskMail => {
                    if (taskMail.Comment == this.stringConstant.notAvailableComment)
                    { taskMail.StatusName = this.stringConstant.notAvailableComment }
                    else {
                        taskMail.StatusName = TaskMailStatus[taskMail.Status];
                    }
                });
            });

        });
    }
}