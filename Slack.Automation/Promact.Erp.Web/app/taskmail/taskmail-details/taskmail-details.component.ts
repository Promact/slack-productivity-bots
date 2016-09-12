import {Component, OnInit} from "@angular/core";
import {ROUTER_DIRECTIVES, Router, ActivatedRoute } from '@angular/router';
import { TaskService }   from '../taskmail.service';
import {taskmailModel} from '../taskmail.model';
import {TaskMailStatus} from '../../enums/TaskMailStatus';
@Component({
    templateUrl: "app/taskmail/taskmail-details/taskmail-details.html",
    directives: [ROUTER_DIRECTIVES]
})
export class TaskMailDetailsComponent {
    taskMails: Array<taskmailModel>;
    public currentPage: number;
    public itemsPerPage: number;
    constructor(private route: ActivatedRoute,private router: Router, private taskService: TaskService) {
        this.taskMails = new Array<taskmailModel>();
    }

    ngOnInit() {
        this.route.params.subscribe(params => {
            let id = +params['id']; // (+) converts string 'id' to a number
            this.currentPage = +params['currentPage'];
            this.itemsPerPage = +params['itemsPerPage'];
            this.taskService.getTaskMailDetailsReport(id).subscribe(taskMails => {
                this.taskMails = taskMails;
                 this.taskMails.forEach(taskMail => {
                    taskMail.StatusName = TaskMailStatus[taskMail.Status];
                 });
                
            });

        });
    }
    getTaskMailList() {
        this.router.navigate(['task/', this.currentPage, this.itemsPerPage]);

    }

}