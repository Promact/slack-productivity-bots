
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { AppComponent } from './app.component';
import { HttpModule, JsonpModule } from '@angular/http';
import { routing } from './app.routes';
import { TaskService } from './taskmail/taskmail.service';
import { HashLocationStrategy, LocationStrategy } from '@angular/common';
import { StringConstant } from './shared/stringConstant';
import { LoaderService } from "./shared/loader.service";
import { TaskMailModule } from './taskmail/taskMail.module';
import { LeaveModule } from './leaveReport/leaveReport.module';
import { ScrumModule } from './ScrumReport/scrumReport.module';
import { AppComponentService } from './appcomponent.service';
import { EmailHashCode } from './shared/emailHashCode';
import { MailSettingModule } from './shared/MailSetting/mailsetting.module';
import { GroupModule } from './Group/group.module';

@NgModule({
    declarations: [AppComponent],
    imports: [
        BrowserModule,
        HttpModule,
        routing,
        GroupModule,
        TaskMailModule,
        LeaveModule,
        ScrumModule,
        FormsModule,
        MailSettingModule,
    ],
    bootstrap: [AppComponent],
    providers: [StringConstant, AppComponentService, LoaderService, { provide: LocationStrategy, useClass: HashLocationStrategy }, EmailHashCode]
})

export class AppModule { }




