import { ModuleWithProviders } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { SendMailComponent } from './mailsetting.sendemail/mailsetting.sendmail.component';

const MailRoutes: Routes = [{
    path: "mailsetting",
    component: SendMailComponent
}];

export const MailSettingRoutes: ModuleWithProviders = RouterModule.forChild(MailRoutes);

