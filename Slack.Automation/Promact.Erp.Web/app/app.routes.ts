
import { Routes, RouterModule, RouterConfig } from '@angular/router';
import { AppComponent } from './app.component';
import { LeaveReportRoutes } from './leaveReport/leaveReport.routes';
import { LeaveReportComponent } from './leaveReport/leaveReport.component';
import { ScrumReportComponent } from './ScrumReport/scrumReport.component';
import { ScrumReportRoutes } from './ScrumReport/scrumReport.routes';

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