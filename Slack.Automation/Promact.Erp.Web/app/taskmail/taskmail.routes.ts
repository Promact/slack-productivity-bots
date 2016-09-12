import { provideRouter, RouterConfig } from '@angular/router';
import {TaskMailComponent} from "./taskmail.component";
import {TaskMailListComponent} from "./taskmail-list/taskmail-list.component";
import {TaskMailDetailsComponent} from "./taskmail-details/taskmail-details.component";
export const TaskMailRoutes: RouterConfig = [{
    path: "task",
    component: TaskMailComponent,
    children: [
        { path: '', component: TaskMailListComponent },
        { path: 'taskdetail/:id/:currentPage/:itemsPerPage', component: TaskMailDetailsComponent },
        { path: ':currentPage/:itemsPerPage', component: TaskMailListComponent }
    ]
}];