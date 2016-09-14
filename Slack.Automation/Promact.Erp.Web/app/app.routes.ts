import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LeaveReportComponent } from './leaveReport/leaveReport.component';
import { ScrumReportComponent } from './ScrumReport/scrumReport.component';
import { ScrumReportRoutes } from './ScrumReport/scrumReport.routes';

const appRoutes: Routes =
    [
        { path: 'Home/AfterLogIn', component: LeaveReportComponent },
        { path: '', component: LeaveReportComponent }
    ]
export const routing: ModuleWithProviders = RouterModule.forRoot(appRoutes);


const appRoutes: Routes = [
    { path: 'Home/AfterLogIn', component: LeaveReportComponent },
    ...LeaveReportRoutes,
    { path: 'leave', component: LeaveReportComponent },
    ...ScrumReportRoutes,
    { path: 'scrum', component: ScrumReportComponent }
        //...TaskMailRoutes,
    //{ path: 'task', component: TaskMailComponent },

];



export const routing = RouterModule.forRoot(appRoutes);