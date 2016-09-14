
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { routing } from './app.routes';
import { AppComponent } from './app.component';
import { SpinnerService } from './shared/spinner.service';
import { HttpModule, XHRBackend } from "@angular/http";
import { TaskMailModule } from './taskmail/taskMail.module';
import { LeaveModule } from './leaveReport/leaveReport.module';
//import { HashLocationStrategy, LocationStrategy } from '@angular/common';
import { SpinnerComponent } from './shared/spinner.component';
import { StringConstant } from './shared/stringConstant';

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
    declarations: [AppComponent, SpinnerComponent],
    imports: [
        BrowserModule,
        HttpModule,
        routing,
        TaskMailModule,
        LeaveModule
    ],
    declarations: [AppComponent, LeaveReportComponent, LeaveReportListComponent, LeaveReportDetailsComponent, ScrumReportComponent, ScrumProjectListComponent, ScrumProjectDetailComponent],
    bootstrap: [AppComponent],
    providers: [SpinnerService, StringConstant]//, { provide: LocationStrategy, useClass: HashLocationStrategy }]
})

export class AppModule { }




