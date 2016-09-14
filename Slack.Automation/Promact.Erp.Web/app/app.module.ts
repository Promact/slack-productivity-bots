
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule }   from '@angular/forms';
import { AppComponent }  from './app.component';
import { HttpModule, JsonpModule } from '@angular/http';
import { routing }       from './app.routes';

import { TaskService }   from './taskmail/taskmail.service';
import { HashLocationStrategy, LocationStrategy } from '@angular/common';
import { StringConstant } from './shared/stringConstant';
import { LoaderService } from "./shared/loader.service";

import { TaskMailModule } from './taskmail/taskMail.module';
import { LeaveModule } from './leaveReport/leaveReport.module';
import { ScrumModule } from './ScrumReport/scrumReport.module';


import { FormsModule }   from '@angular/forms';
import { AppComponent }  from './app.component';
import { HttpModule, JsonpModule } from '@angular/http';
import { routing }       from './app.routes';
import { LeaveReportComponent } from './leaveReport/leaveReport.component';
import { LeaveReportListComponent } from './leaveReport/leaveReport-List/leaveReport-List.component';
import { LeaveReportDetailsComponent } from './leaveReport/leaveReport-Details/leaveReport-Details.component';
import { LeaveReportService } from './leaveReport/leaveReport.service';
import { ScrumReportComponent } from './ScrumReport/scrumReport.component';
import { ScrumReportService } from './ScrumReport/scrumReport.service';
import { ScrumProjectListComponent } from './ScrumReport/scrumProject-List/scrumProject-List.component';
import { ScrumProjectDetailComponent } from './ScrumReport/scrumProject-Details/scrumProject-Details.component';



@NgModule({
    declarations: [AppComponent],
    imports: [
        BrowserModule,
        HttpModule,
        routing,
        TaskMailModule,
        LeaveModule,
        ScrumModule
    ],
    declarations: [AppComponent, LeaveReportComponent, LeaveReportListComponent, LeaveReportDetailsComponent, ScrumReportComponent, ScrumProjectListComponent, ScrumProjectDetailComponent],
    bootstrap: [AppComponent],
    providers: [StringConstant, LoaderService,{ provide: LocationStrategy, useClass: HashLocationStrategy }]
})

export class AppModule { }




