import { NgModule } from "@angular/core";
import { MailSettingComponent } from "./mailsetting.component";
import { MailSettingService } from "./mailsetting.service";
import { SharedModule } from '../shared.module';

@NgModule({
    imports: [
        SharedModule
    ],
    declarations: [
        MailSettingComponent
    ],
    providers: [
        MailSettingService
    ]
})
export class MailSettingModule { }