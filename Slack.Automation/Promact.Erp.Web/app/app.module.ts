
import { NgModule }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule }   from '@angular/forms';
import { AppComponent }  from './app.component';
import { HttpModule, JsonpModule } from '@angular/http';
import { routing }       from './app.routes';
import { LeaveReportComponent } from './leaveReport/leaveReportList/leaveReport';
import { LeaveReportDetailsComponent } from './leaveReport/leaveReportDetails/leaveReportDetails';
import { LeaveReportService } from './leaveReport/leaveReport.service';
import {HttpService} from "./http.service";
import { TaskService }   from './taskmail/taskmail.service';




@NgModule({
    imports: [
        BrowserModule,
        FormsModule,
        HttpModule,
        JsonpModule,
        routing
    ],
    declarations: [AppComponent, LeaveReportComponent, LeaveReportDetailsComponent],
    bootstrap: [AppComponent],
    providers: [HttpService, LeaveReportService, TaskService]
})

export class AppModule { }