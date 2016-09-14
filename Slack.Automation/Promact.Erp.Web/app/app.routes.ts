
import { Routes, RouterModule, RouterConfig } from '@angular/router';
import { AppComponent } from './app.component';
import { LeaveReportRoutes } from './leaveReport/leaveReport.routes';
import { LeaveReportComponent } from './leaveReport/leaveReport.component';
import { TaskMailRoutes } from './taskmail/taskmail.routes';
import { TaskMailComponent } from './taskmail/taskmail.component';
import { ScrumReportComponent } from './ScrumReport/scrumReport.component';
import { ScrumReportRoutes } from './ScrumReport/scrumReport.routes';

const appRoutes: Routes = [
    { path: 'Home/AfterLogIn', component: LeaveReportComponent },
    ...LeaveReportRoutes,
    { path: 'leave', component: LeaveReportComponent },
     ...TaskMailRoutes,
     { path: 'task', component: TaskMailComponent },
     ...ScrumReportRoutes,
     { path: 'scrum', component: ScrumReportComponent }

];

export const routing = RouterModule.forRoot(appRoutes);
    //{
    //    path: 'report',
    //    component: LeaveReportComponent
    //},
    //{
    //    path: 'detail/:id',
    //    component: LeaveReportDetailsComponent 
    //},
    //{
    //    path: 'Home/AfterLogIn',
    //    redirectTo: '/report',
    //    pathMatch: 'full'
    //},

    //{
    //    path: 'task',
    //    component: TaskMailListComponent
    //},
    //{
    //    path: 'testdetail/:id',
    //    component: TaskMailDetailsComponent
    //},




//export const routing = RouterModule.forRoot(appRoutes);