import { Component, OnInit } from '@angular/core';
import {ROUTER_DIRECTIVES, Router, ActivatedRoute } from '@angular/router';
import { TaskService }   from '../taskmail.service';
import {taskmailModel} from '../taskmail.model';
import {TaskMailStatus} from '../../enums/TaskMailStatus';
import { FilterPipe } from '../filter.pipe';
import { DatePipe } from '@angular/common';
import { PaginationModule } from 'ng2-bootstrap/components/pagination';

@Component({
    templateUrl: "app/taskmail/taskmail-list/taskmail-list.html",
    directives: [ROUTER_DIRECTIVES],
    pipes: [FilterPipe]
})
export class TaskMailListComponent {
   // public totalItems: number = 64;
   // public currentPage: number = 1;

   // //public maxSize: number = 5;
   //// public bigTotalItems: number = 175;
   //// public bigCurrentPage: number = 1;

   // public setPage(pageNo: number): void {
   //     this.currentPage = pageNo;
   // };

   // public pageChanged(event: any): void {
   //     console.log('Page changed to: ' + event.page);
   //     console.log('Number items per page: ' + event.itemsPerPage);
   // };
    taskMails: Array<taskmailModel>;
    private UserName: string;
    private CreatedOn: Date;

    public totalItems: number;
    public currentPage: number;
    public maxSize: number;
    public itemsPerPage: number;
    public viewby: number;
    constructor(private route: ActivatedRoute,private router: Router, private taskService: TaskService) {
        this.taskMails = new Array<taskmailModel>();
        this.currentPage = 0;
        this.itemsPerPage = 2;

    }
    public setPage(pageNo: number): void {
        this.currentPage = pageNo;
    };
    public onChange(viewby: number)
    {
        this.currentPage = 1;
        this.itemsPerPage = viewby;
        this.gettaskMailReport(this.currentPage, this.itemsPerPage)

    };
    public pageChanged(event: any): void {
        //console.log('Page changed to: ' + event.page);
        //console.log('Number items per page: ' + event.itemsPerPage);
        this.route.params.subscribe(params => {
            this.currentPage = event.page; // (+) converts string 'id' to a number
            this.gettaskMailReport(this.currentPage, this.itemsPerPage);
            this.setPage(this.currentPage);
        });

    };
    //gettaskMailReport() {
    //    this.taskService.getTaskMailReport().subscribe((taskMails: Array<taskmailModel>) => {
    //        this.taskMails = taskMails;
    //        this.taskMails.forEach(taskMail => {
    //            var datePipe = new DatePipe();
    //            taskMail.CreatedOns = datePipe.transform(taskMail.CreatedOn, 'yyyy-MM-dd');
    //        });
    //        this.totalItems = taskMails.length;
    //    }, err => {
    //  });
    //}
    gettaskMailReport(currentPage, itemsPerPage) {
        this.route.params.subscribe(params => {
            this.currentPage = currentPage;
            this.itemsPerPage = itemsPerPage;
            this.taskService.getTaskMailReport(this.currentPage, this.itemsPerPage).subscribe(taskMails => {
                this.taskMails = taskMails;
                this.taskMails.forEach(taskMail => {
                var datePipe = new DatePipe();
                taskMail.CreatedOns = datePipe.transform(taskMail.CreatedOn, 'yyyy-MM-dd');
                this.totalItems = taskMail.TotalItems;
    
            });
                this.setPage(this.currentPage);           
            });

        });
      //  this.currentPage = 1;
      //  this.taskService.getTaskMailReport(this.currentPage).subscribe((taskMails: Array<taskmailModel>) => {
      //      this.taskMails = taskMails;
      //      this.taskMails.forEach(taskMail => {
      //          var datePipe = new DatePipe();
      //          taskMail.CreatedOns = datePipe.transform(taskMail.CreatedOn, 'yyyy-MM-dd');
      //      });
      //      this.totalItems = taskMails.length;
      //  }, err => {
      //});
    }
    ngOnInit() {
        this.route.params.subscribe(params => {
            this.currentPage = +params['currentPage']; // (+) conve
            if (isNaN(this.currentPage)) {
                this.viewby = this.itemsPerPage;
                this.gettaskMailReport(1, this.itemsPerPage);
            }
            else {
                this.currentPage = +params['currentPage'];
                this.itemsPerPage = +params['itemsPerPage'];
                this.setPage(this.currentPage);
                this.gettaskMailReport(this.currentPage, this.itemsPerPage);
                this.viewby = this.itemsPerPage;
            }
        });
        
    }
    taskMailDetails(Id) {
        this.router.navigate(['task/taskdetail', Id, this.currentPage, this.itemsPerPage]);
        this.viewby = this.itemsPerPage;
    }
}