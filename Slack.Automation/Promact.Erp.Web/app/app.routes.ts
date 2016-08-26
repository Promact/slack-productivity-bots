
import { Routes, RouterModule } from '@angular/router';
import { LeaveReportComponent } from './leaveReport/leaveReportList/leaveReport';
import { LeaveReportDetailsComponent } from './leaveReport/leaveReportDetails/leaveReportDetails';

const appRoutes: Routes = [
    {
        path: 'report',
        component: LeaveReportComponent
    },
    {
        path: 'detail/:id',
        component: LeaveReportDetailsComponent 
    },
    {
        path: 'Home/AfterLogIn',
        redirectTo: '/report',
        pathMatch: 'full'
    } 
];


export const routing = RouterModule.forRoot(appRoutes);