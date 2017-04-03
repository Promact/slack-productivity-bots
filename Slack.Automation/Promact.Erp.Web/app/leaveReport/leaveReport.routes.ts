import { ModuleWithProviders } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { LeaveReportComponent } from "./leaveReport.component";
import { LeaveReportListComponent } from './leaveReport-List/leaveReport-List.component';
import { LeaveReportDetailsComponent } from './leaveReport-Details/leaveReport-Details.component';
import { MailSettingComponent } from '../shared/MailSetting/mailsetting.component';

const LeaveReportRoutes: Routes = [{
    path: "leave",
    component: LeaveReportComponent,
    children: [{ path: '', component: LeaveReportListComponent },
        { path: 'detail/:id', component: LeaveReportDetailsComponent },
        { path: 'mailsetting', component: MailSettingComponent}]
}];
export const leaveReportRoutes: ModuleWithProviders = RouterModule.forChild(LeaveReportRoutes);











