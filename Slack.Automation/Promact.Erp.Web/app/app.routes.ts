//import { provideRouter, RouterConfig }  from '@angular/router';

//import { AppComponent } from './app.component';
//const routes: RouterConfig = [
//    {
//        path: 'reports',
//        component: AppComponent
//    },
//    //{
//    //    path: '',
//    //    redirectTo: '/main',
//    //    pathMatch: 'full'
//    //},
//];

//export const appRouterProviders = [
//    provideRouter(routes)
//];


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
        path: '',
        redirectTo: '/report',
        pathMatch: 'full'
    }  
];


export const routing = RouterModule.forRoot(appRoutes);