import { ModuleWithProviders } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { TaskMailComponent } from "./taskmail.component";
import { TaskMailListComponent } from "./taskmail-list/taskmail-list.component";
import { TaskMailDetailsComponent } from "./taskmail-details/taskmail-details.component";
import { MailSettingComponent } from '../shared/MailSetting/mailsetting.component';

const TaskMailRoutes: Routes = [{
    path: "task",
    component: TaskMailComponent,
    children: [{ path: '', component: TaskMailListComponent },
        { path: 'taskdetail/:UserId/:UserRole/:UserName', component: TaskMailDetailsComponent },
        { path: 'mailsetting', component: MailSettingComponent }]
}];
export const taskMailRoutes: ModuleWithProviders = RouterModule.forChild(TaskMailRoutes);