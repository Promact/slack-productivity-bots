
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
import { AppComponentService } from './appcomponent.service';
import { MailSettingComponent } from './shared/mailsetting/mailsetting.component';


@NgModule({
    declarations: [AppComponent, MailSettingComponent],
    imports: [
        BrowserModule,
        HttpModule,
        routing,
        TaskMailModule,
        LeaveModule,
        ScrumModule
    ],
    bootstrap: [AppComponent],
    providers: [StringConstant, AppComponentService, LoaderService, { provide: LocationStrategy, useClass: HashLocationStrategy }]
})

export class AppModule { }




