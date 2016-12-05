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
    public IsMaxDate: boolean;
    public IsMinDate: boolean;
    public SelectedDate: string;
    public MaxDate: string;
    public MinDate: string;
    public IsHide: boolean;
    constructor(private route: ActivatedRoute, private router: Router, private taskService: TaskService, private stringConstant: StringConstant, private loader: LoaderService) {
    }
    ngOnInit() {
        this.getTaskMailDetails();
    }
    getTaskMailDetails() {
        this.loader.loader = true;
        this.route.params.subscribe(params => {
            if (params[this.stringConstant.userRole] === this.stringConstant.RoleAdmin) {
                this.IsHide = false;
            }
            else {
                this.IsHide = true;
            }
            this.IsMaxDate = true;
            this.taskService.getTaskMailDetailsReport(params[this.stringConstant.paramsUserId], params[this.stringConstant.userRole], params[this.stringConstant.paramsUserName]).subscribe(taskMails => {
                this.taskMail = taskMails;
                let datePipeMinDate = new DatePipe(this.stringConstant.medium);
                this.MinDate = datePipeMinDate.transform(this.taskMail[0].MinDate, this.stringConstant.dateDefaultFormat);
                if (this.MinDate === this.stringConstant.defaultDate) {
                    this.IsMinDate = true;
                }
                let datePipeMaxDate = new DatePipe(this.stringConstant.medium);
                this.MaxDate = datePipeMaxDate.transform(this.taskMail[0].MaxDate, this.stringConstant.dateDefaultFormat);
                this.taskMail.forEach(taskmails => {
                    let datePipe = new DatePipe(this.stringConstant.medium);
                    taskmails.CreatedOns = datePipe.transform(taskmails.CreatedOn, this.stringConstant.dateFormat);
                    taskmails.TaskMails.forEach(taskMail => {
                        taskMail.StatusName = TaskMailStatus[taskMail.Status];
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
        this.SelectedDate = this.stringConstant.empty;
       this.taskService.getTaskMailDetailsReportPreviousDate(UserName,UserId,UserRole, CreatedOn).subscribe(taskMails => {
           this.taskMail = taskMails;
           if (this.taskMail[0].MinDate === this.taskMail[0].CreatedOn) {
               this.IsMinDate = true;
                }
           this.taskMail.forEach(taskmails => {
               let datePipe = new DatePipe(this.stringConstant.medium);
                    taskmails.CreatedOns = datePipe.transform(taskmails.CreatedOn, this.stringConstant.dateFormat);
                    taskmails.TaskMails.forEach(taskMail => {
                       taskMail.StatusName = TaskMailStatus[taskMail.Status];
                    });
                    });
                });
        });
       this.IsMaxDate = false;
    }
    getTaskMailNext(UserName, UserId, UserRole, CreatedOn) {
        this.SelectedDate = this.stringConstant.empty;
        this.taskService.getTaskMailDetailsReportNextDate(UserName, UserId, UserRole, CreatedOn).subscribe(taskMails => {
            this.taskMail = taskMails;
            if (this.taskMail[0].MaxDate === this.taskMail[0].CreatedOn) {
                this.IsMaxDate = true;
            }
            this.IsMinDate = false;
            this.taskMail.forEach(taskmails => {
                let datePipe = new DatePipe(this.stringConstant.medium);
                taskmails.CreatedOns = datePipe.transform(taskmails.CreatedOn, this.stringConstant.dateFormat);
                taskmails.TaskMails.forEach(taskMail => {
                        taskMail.StatusName = TaskMailStatus[taskMail.Status];
                   
                });
            });
            
        });
    }
    getTaskMailForSelectedDate(UserName, UserId, UserRole, CreatedOn, SelectedDate) {
        this.taskService.getTaskMailDetailsReportSelectedDate(UserName, UserId, UserRole, CreatedOn, SelectedDate).subscribe(taskMails => {
            this.taskMail = taskMails;
            if (this.taskMail[0].MaxDate === this.taskMail[0].CreatedOn) {
                this.IsMaxDate = true;
                this.IsMinDate = false;
            }
            if (this.taskMail[0].MinDate === this.taskMail[0].CreatedOn) {
                this.IsMaxDate = false;
                this.IsMinDate = true;
            }
            this.taskMail.forEach(taskmails => {
                let datePipe = new DatePipe(this.stringConstant.medium);
                taskmails.CreatedOns = datePipe.transform(taskmails.CreatedOn, this.stringConstant.dateFormat);
                taskmails.TaskMails.forEach(taskMail => {
                       taskMail.StatusName = TaskMailStatus[taskMail.Status];
                });
            });

        });
    }
}