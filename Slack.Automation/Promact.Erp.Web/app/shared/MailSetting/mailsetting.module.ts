import { NgModule } from "@angular/core";
import { MailSettingComponent } from "./mailsetting.component";
import { MailSettingService } from "./mailsetting.service";
import { SharedModule } from '../shared.module';
import { MaterialAutoSelectChip } from '../angular-material-chip-autoselect.service';

@NgModule({
    imports: [
        SharedModule
    ],
    declarations: [
        MailSettingComponent
    ],
    providers: [
        MailSettingService,
        MaterialAutoSelectChip
    ]
})
export class MailSettingModule { }