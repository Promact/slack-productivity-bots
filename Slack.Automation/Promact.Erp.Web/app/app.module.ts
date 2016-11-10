
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { routing } from './app.routes';
import { AppComponent } from './app.component';
import { HttpModule, XHRBackend } from "@angular/http";
import { TaskMailModule } from './taskmail/taskMail.module';
import { LeaveModule } from './leaveReport/leaveReport.module';
import { HashLocationStrategy, LocationStrategy } from '@angular/common';
import { StringConstant } from './shared/stringConstant';
import { LoaderService } from "./shared/loader.service";




@NgModule({
    declarations: [AppComponent],
    imports: [
        BrowserModule,
        HttpModule,
        routing,
        TaskMailModule,
        LeaveModule
    ],
    bootstrap: [AppComponent],
    providers: [StringConstant, LoaderService,{ provide: LocationStrategy, useClass: HashLocationStrategy }]
})

export class AppModule { }




