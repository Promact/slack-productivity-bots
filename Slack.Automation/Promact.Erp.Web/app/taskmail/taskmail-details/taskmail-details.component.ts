import {Component, OnInit} from "@angular/core";
import {Router, ActivatedRoute } from '@angular/router';
import { TaskService }   from '../taskmail.service';
import {taskmailModel} from '../taskmail.model';
import {taskmailuserModel} from '../taskmailuser.model';
import {TaskMailStatus} from '../../enums/TaskMailStatus';
import { DatePipe } from '@angular/common';
import { StringConstant } from '../../shared/stringConstant';
import { LoaderService } from '../../shared/loader.service';

@Component({
    selector: 'date-pipe',
    templateUrl: "app/taskmail/taskmail-details/taskmail-details.html",
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
                this.taskMailUser = taskMailUser;
                var datePipeMinDate = new DatePipe("medium");
                this.isMinDate = datePipeMinDate.transform(this.taskMailUser[0].MinDate, this.stringConstant.dateDefaultFormat);
                if (this.isMinDate == this.stringConstant.defaultDate)
                { this.isMin = true; }
                var datePipeMaxDate = new DatePipe("medium");
                this.isMaxDate = datePipeMaxDate.transform(this.taskMailUser[0].MaxDate, this.stringConstant.dateDefaultFormat);
                this.taskMailUser.forEach(taskmailuser => {
                    var datePipe = new DatePipe("medium");
                    taskmailuser.CreatedOns = datePipe.transform(taskmailuser.CreatedOn, this.stringConstant.dateFormat);
                    taskmailuser.TaskMails.forEach(taskMail => {
                        if (taskMail.Comment == this.stringConstant.notAvailableComment)
                        { taskMail.StatusName = this.stringConstant.notAvailableComment }
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
           if (this.taskMailUser[0].MinDate == this.taskMailUser[0].CreatedOn) {
                    this.isMin = true;
                }
                this.taskMailUser.forEach(taskmailuser => {
                    var datePipe = new DatePipe("medium");
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
            if (this.taskMailUser[0].MaxDate == this.taskMailUser[0].CreatedOn) {
                this.isMax = true;
            }
            this.isMin = false;
            this.taskMailUser.forEach(taskmailuser => {
                var datePipe = new DatePipe("medium");
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
            if (this.taskMailUser[0].MaxDate == this.taskMailUser[0].CreatedOn) {
                this.isMax = true;
                this.isMin = false;
            }
            if (this.taskMailUser[0].MinDate == this.taskMailUser[0].CreatedOn) {
                this.isMax = false;
                this.isMin = true;
            }
            this.taskMailUser.forEach(taskmailuser => {
                var datePipe = new DatePipe("medium");
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