import { provideRouter, RouterConfig } from '@angular/router';
import { ScrumReportComponent } from './scrumReport.component';
import { ScrumProjectListComponent } from './scrumProject-List/scrumProject-List.component';
import { ScrumProjectDetailComponent } from './scrumProject-Details/scrumProject-Details.component';


export const ScrumReportRoutes: RouterConfig = [{
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
