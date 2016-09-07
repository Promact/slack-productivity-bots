
import { Routes, RouterModule, RouterConfig } from '@angular/router';
import { AppComponent } from './app.component';
import { LeaveReportRoutes } from './leaveReport/leaveReport.routes';
import { LeaveReportComponent } from './leaveReport/leaveReport.component';

const appRoutes: Routes = [
    { path: 'Home/AfterLogIn', component: LeaveReportComponent },
    ...LeaveReportRoutes,
    { path: 'leave', component: LeaveReportComponent }
        //...TaskMailRoutes,
    //{ path: 'task', component: TaskMailComponent },

];



export const routing = RouterModule.forRoot(appRoutes);