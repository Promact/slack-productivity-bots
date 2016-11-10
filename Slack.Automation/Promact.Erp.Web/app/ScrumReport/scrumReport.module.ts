import { NgModule } from '@angular/core';
import { ScrumReportComponent } from './scrumReport.component';
import { ScrumReportService } from './scrumReport.service';
import { ScrumProjectListComponent } from './scrumProject-List/scrumProject-List.component';
import { ScrumProjectDetailComponent } from './scrumProject-Details/scrumProject-Details.component';
import { scrumReportRoutes } from "./scrumReport.routes";
import { SharedModule } from '../shared/shared.module';

@NgModule({
    imports: [
        scrumReportRoutes,
        SharedModule
    ],
    declarations: [
        ScrumReportComponent,
        ScrumProjectListComponent,
        ScrumProjectDetailComponent,
    ],
    providers: [
        ScrumReportService
    ]
})

export class ScrumModule { }

