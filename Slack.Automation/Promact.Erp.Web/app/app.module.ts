
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
import { SpinnerComponent} from './shared/spinner.component';
import { SpinnerService} from './shared/spinner.service';
import { StringConstant } from './shared/stringConstant';
//import { HashLocationStrategy, LocationStrategy } from '@angular/common';
import { ScrumReportComponent } from './ScrumReport/scrumReport.component';
import { ScrumReportService } from './ScrumReport/scrumReport.service';
import { ScrumProjectListComponent } from './ScrumReport/scrumProject-List/scrumProject-List.component';
import { ScrumProjectDetailComponent } from './ScrumReport/scrumProject-Details/scrumProject-Details.component';



@NgModule({
    imports: [
        BrowserModule,
        FormsModule,
        HttpModule,
        JsonpModule,
        routing,
      
    ],
    declarations: [AppComponent, LeaveReportComponent, LeaveReportListComponent, LeaveReportDetailsComponent, SpinnerComponent, ScrumReportComponent, ScrumProjectListComponent, ScrumProjectDetailComponent],
    bootstrap: [AppComponent],
    providers: [LeaveReportService, TaskService, SpinnerService, ScrumReportService, StringConstant]//, { provide: LocationStrategy, useClass: HashLocationStrategy }]
})

export class AppModule { }