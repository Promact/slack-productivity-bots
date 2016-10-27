import { NgModule } from "@angular/core";
import { TaskMailComponent } from "./taskmail.component";
import { TaskMailListComponent } from "./taskmail-list/taskmail-list.component";
import { TaskMailDetailsComponent } from "./taskmail-details/taskmail-details.component";
import { taskMailRoutes } from './taskmail.routes';
import { TaskService } from "./taskmail.service";
import { SharedModule } from '../shared/shared.module';
import { DatePipe } from '@angular/common';

@NgModule({
    imports: [
        taskMailRoutes,
        SharedModule
    ],
    declarations: [
        TaskMailComponent,
        TaskMailListComponent,
        TaskMailDetailsComponent,
    ],
    providers: [
        TaskService,
        DatePipe
    ]
})
export class TaskMailModule { }