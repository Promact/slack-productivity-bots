import {Component, OnInit} from "@angular/core";
import {ROUTER_DIRECTIVES, Router, ActivatedRoute } from '@angular/router';
import { TaskService }   from '../taskmail.service';
import {taskmailModel} from '../taskmail.model';
import {taskmailuserModel} from '../taskmailuser.model';
import {TaskMailStatus} from '../../enums/TaskMailStatus';
import { DatePipe } from '@angular/common';
import { SpinnerService} from '../../spinner.service';

@Component({
    templateUrl: "app/taskmail/taskmail-details/taskmail-details.html",
    directives: [ROUTER_DIRECTIVES]
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
    constructor(private route: ActivatedRoute, private router: Router, private taskService: TaskService, private spinner: SpinnerService) {
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
            this.UserId = params['UserId']; // (+) converts string 'id' to a number
            this.UserRole = params['UserRole'];
            this.UserName = params['UserName'];
            if (this.UserRole == "Admin")
            { this.isHide = false; } else { this.isHide = true; }
            this.isMax = true;
            this.taskService.getTaskMailDetailsReport(this.UserId, this.UserRole, this.UserName).subscribe(taskMailUser => {
                this.taskMailUser = taskMailUser;
                var datePipeMinDate = new DatePipe();
                this.isMinDate = datePipeMinDate.transform(this.taskMailUser[0].IsMin, 'yyyy-MM-dd');
                if (this.isMinDate == "1-01-01")
                { this.isMin = true; }
                var datePipeMaxDate = new DatePipe();
                this.isMaxDate = datePipeMaxDate.transform(this.taskMailUser[0].IsMax, 'yyyy-MM-dd');
                this.taskMailUser.forEach(taskmailuser => {
                    var datePipe = new DatePipe();
                    taskmailuser.CreatedOns = datePipe.transform(taskmailuser.CreatedOn, 'dd-MM-yyyy');
                    taskmailuser.TaskMails.forEach(taskMail => {
                        if (taskMail.Comment == "Not Available")
                        { taskMail.StatusName = "Not Available" }
                        else {
                            taskMail.StatusName = TaskMailStatus[taskMail.Status];
                        }
                    });
                });
            });
        });
    }
    getTaskMailList() {
        this.router.navigate(['/task']);
    }
    getTaskMailPrevious(UserName,UserId,UserRole,CreatedOn) {
       this.taskService.getTaskMailDetailsReportPreviousDate(UserName,UserId,UserRole, CreatedOn).subscribe(taskMailUser => {
           this.taskMailUser = taskMailUser;
                if (this.taskMailUser[0].IsMin == this.taskMailUser[0].CreatedOn) {
                    this.isMin = true;
                }
                this.taskMailUser.forEach(taskmailuser => {
                    var datePipe = new DatePipe();
                    taskmailuser.CreatedOns = datePipe.transform(taskmailuser.CreatedOn, 'dd-MM-yyyy');
                    taskmailuser.TaskMails.forEach(taskMail => {
                        if (taskMail.Comment == "Not Available")
                        { taskMail.StatusName ="Not Available" }
                        else {
                            taskMail.StatusName = TaskMailStatus[taskMail.Status];
                        }
                    });
                });
        });
        this.isMax = false;
    }
    getTaskMailNext(UserName,UserId,UserRole,CreatedOn) {
        this.taskService.getTaskMailDetailsReportNextDate(UserName, UserId, UserRole, CreatedOn).subscribe(taskMailUser => {
            this.taskMailUser = taskMailUser;
            if (this.taskMailUser[0].IsMax == this.taskMailUser[0].CreatedOn) {
                this.isMax = true;
            }
            this.isMin = false;
            this.taskMailUser.forEach(taskmailuser => {
                var datePipe = new DatePipe();
                taskmailuser.CreatedOns = datePipe.transform(taskmailuser.CreatedOn, 'dd-MM-yyyy');
                taskmailuser.TaskMails.forEach(taskMail => {
                    if (taskMail.Comment == "Not Available")
                    { taskMail.StatusName = "Not Available" }
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
                taskmailuser.CreatedOns = datePipe.transform(taskmailuser.CreatedOn, 'dd-MM-yyyy');
                taskmailuser.TaskMails.forEach(taskMail => {
                    if (taskMail.Comment == "Not Available")
                    { taskMail.StatusName = "Not Available" }
                    else {
                        taskMail.StatusName = TaskMailStatus[taskMail.Status];
                    }
                });
            });

        });
    }
}