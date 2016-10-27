import { NgModule } from "@angular/core";
import {LeaveReportComponent } from "./leaveReport.component";
import { LeaveReportListComponent } from './leaveReport-List/leaveReport-List.component';
import { LeaveReportDetailsComponent } from './leaveReport-Details/leaveReport-Details.component';
import { LeaveReportService } from './leaveReport.service';
import { leaveReportRoutes } from "./leaveReport.routes";
import { SharedModule } from '../shared/shared.module';



@NgModule({
    imports: [
        leaveReportRoutes,
        SharedModule
    ],
    declarations: [
        LeaveReportComponent,
        LeaveReportListComponent,
        LeaveReportDetailsComponent,
    ],
    providers: [
        LeaveReportService
    ]
})
export class LeaveModule { }