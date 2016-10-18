
import { NgModule }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule }   from '@angular/forms';
import { AppComponent }  from './app.component';
import { HttpModule, JsonpModule } from '@angular/http';
import { routing }       from './app.routes';
import { LeaveReportComponent } from './leaveReport/leaveReport.component';
import { LeaveReportListComponent } from './leaveReport/leaveReport-List/leaveReport-List.component';
import { LeaveReportDetailsComponent } from './leaveReport/leaveReport-Details/leaveReport-Details.component';
import { LeaveReportService } from './leaveReport/leaveReport.service';
import { TaskService }   from './taskmail/taskmail.service';

import { StringConstant } from './shared/stringConstant';
import { LoaderService } from "./shared/loader.service";




@NgModule({
    imports: [
        BrowserModule,
        FormsModule,
        HttpModule,
        JsonpModule,
        routing,
      
    ],
    declarations: [AppComponent, LeaveReportComponent, LeaveReportListComponent, LeaveReportDetailsComponent],
    bootstrap: [AppComponent],
    providers: [LeaveReportService, TaskService, StringConstant, LoaderService]//, { provide: LocationStrategy, useClass: HashLocationStrategy }]
})

export class AppModule { }