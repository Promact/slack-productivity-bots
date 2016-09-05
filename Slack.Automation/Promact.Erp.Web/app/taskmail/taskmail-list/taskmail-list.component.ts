import { Component, OnInit } from '@angular/core';
import {ROUTER_DIRECTIVES, Router } from '@angular/router';
import { TaskService }   from '../taskmail.service';
import {taskmailModel} from '../taskmail.model';
import {TaskMailStatus} from '../../enums/TaskMailStatus';
import { FilterPipe } from '../filter.pipe';
import { DatePipe } from '@angular/common';
@Component({
    templateUrl: "app/taskmail/taskmail-list/taskmail-list.html",
    directives: [ROUTER_DIRECTIVES],
    pipes: [FilterPipe]
})
export class TaskMailListComponent {
    taskMails: Array<taskmailModel>;
    private UserName: string;
    private CreatedOn: Date;
    constructor(private router: Router, private taskService: TaskService) {
        this.taskMails = new Array<taskmailModel>();
    }
    gettaskMailReport() {
        this.taskService.getTaskMailReport().subscribe((taskMails: Array<taskmailModel>) => {
            this.taskMails = taskMails;
            this.taskMails.forEach(taskMail => {
                var datePipe = new DatePipe();
                taskMail.CreatedOns = datePipe.transform(taskMail.CreatedOn, 'yyyy-MM-dd');
                //taskMail.CreatedOns = taskMail.CreatedOn.getFullYear().toString() + "-" + taskMail.CreatedOn.getMonth().toString() + "-" + taskMail.CreatedOn.getDay().toString();
            });
        }, err => {
      });
    }
    ngOnInit() {
        this.gettaskMailReport();
    }
    taskMailDetails(Id) {
        this.router.navigate(['task/taskdetail', Id]);
    }
}