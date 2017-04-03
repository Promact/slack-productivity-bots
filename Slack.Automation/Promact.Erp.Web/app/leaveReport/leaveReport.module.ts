import { NgModule } from "@angular/core";
import {LeaveReportComponent } from "./leaveReport.component";
import { LeaveReportListComponent } from './leaveReport-List/leaveReport-List.component';
import { LeaveReportDetailsComponent } from './leaveReport-Details/leaveReport-Details.component';
import { LeaveReportService } from './leaveReport.service';
import { leaveReportRoutes } from "./leaveReport.routes";
import { SharedModule } from '../shared/shared.module';
import { FilterPipe } from './filter.pipe';
import { JsonToPdfService } from '../shared/jsontopdf.service';
import { StringConstant } from '../shared/stringConstant';

@NgModule({
    imports: [
        leaveReportRoutes,
        SharedModule
    ],
    declarations: [
        LeaveReportComponent,
        LeaveReportListComponent,
        LeaveReportDetailsComponent,
        FilterPipe,
    ],
    providers: [
        LeaveReportService,
        JsonToPdfService,
        StringConstant
    ],
})
export class LeaveModule { }