import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LeaveReportComponent } from './leaveReport/leaveReport.component';
import { ScrumReportComponent } from './ScrumReport/scrumReport.component';
import { ScrumReportRoutes } from './ScrumReport/scrumReport.routes';



const appRoutes: Routes =
    [
       { path: '', component: LeaveReportComponent }
    ]
export const routing: ModuleWithProviders = RouterModule.forRoot(appRoutes);
