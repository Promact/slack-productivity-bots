import { ModuleWithProviders } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { ScrumReportComponent } from './scrumReport.component';
import { ScrumProjectListComponent } from './scrumProject-List/scrumProject-List.component';
import { ScrumProjectDetailComponent } from './scrumProject-Details/scrumProject-Details.component';


const ScrumReportRoutes: Routes = [{
    path: "scrum",
    component: ScrumReportComponent,
    children: [
        {
            path: '',
            component: ScrumProjectListComponent
        },
        {
            path: 'detail/:id',
            component: ScrumProjectDetailComponent
        },
    ]
}];

export const scrumReportRoutes: ModuleWithProviders = RouterModule.forChild(ScrumReportRoutes);

