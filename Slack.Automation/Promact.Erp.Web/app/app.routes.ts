
import { Routes, RouterModule } from '@angular/router';
import { LeaveReportListComponent } from './leaveReport/leaveReport-List/leaveReport-List.component';
import { LeaveReportDetailsComponent } from './leaveReport/leaveReport-Details/leaveReport-Details.component';

const appRoutes: Routes = [
    {
        path: 'report',
        component: LeaveReportListComponent
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