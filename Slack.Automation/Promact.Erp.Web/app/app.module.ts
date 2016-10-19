
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
import { HashLocationStrategy, LocationStrategy } from '@angular/common';



@NgModule({
    declarations: [AppComponent, SpinnerComponent],
    imports: [
        BrowserModule,
        HttpModule,
        routing,
        TaskMailModule,
        LeaveModule
    ],
    bootstrap: [AppComponent],
    providers: [LeaveReportService, TaskService, SpinnerService, , StringConstant, { provide: LocationStrategy, useClass: HashLocationStrategy }],
})

export class AppModule { }




