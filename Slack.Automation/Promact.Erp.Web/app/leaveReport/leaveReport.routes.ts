import { provideRouter, RouterConfig } from '@angular/router';
import {LeaveReportComponent} from "./leaveReport.component";
import { LeaveReportListComponent } from './leaveReport-List/leaveReport-List.component';
import { LeaveReportDetailsComponent } from './leaveReport-Details/leaveReport-Details.component';

export const LeaveReportRoutes: RouterConfig = [{
    path: "leave",
    component: LeaveReportComponent,
    children: [
        {
            path: '',
            component: LeaveReportListComponent
        },
        {
            path: 'detail/:id',
            component: LeaveReportDetailsComponent
        },
    ]
}];
