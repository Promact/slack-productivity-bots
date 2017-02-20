import { NgModule } from '@angular/core';
import { MailSettingRoutes } from './mailsetting.routes';
import { MailSettingService } from './mailsetting.service';
import { MailSettingComponent } from './mailsetting.component';
import { SendMailComponent } from './mailsetting.sendemail/mailsetting.sendmail.component';
import { SharedModule } from '../shared/shared.module';

@NgModule({
    imports: [
        MailSettingRoutes,
        SharedModule
    ],
    declarations: [
        MailSettingComponent,
        SendMailComponent
    ],
    providers: [
        MailSettingService
    ]
})

export class MailSettingModule { }
