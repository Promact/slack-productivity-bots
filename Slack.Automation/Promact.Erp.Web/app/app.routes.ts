﻿import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LeaveReportComponent } from './leaveReport/leaveReport.component';



const appRoutes: Routes =
    [
        { path: 'Home/AfterLogIn', component: LeaveReportComponent },
        { path: '', component: LeaveReportComponent }
    ]
export const routing: ModuleWithProviders = RouterModule.forRoot(appRoutes);  