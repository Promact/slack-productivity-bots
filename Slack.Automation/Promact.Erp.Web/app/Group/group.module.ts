import { NgModule } from "@angular/core";
import { SharedModule } from '../shared/shared.module';
import { groupReportRoutes } from "./group.routes";
import { GroupComponent } from "./group.component";
import { GroupAddComponent } from "./GroupAdd/groupAdd.component";
import { GroupEditComponent } from "./GroupAdd/groupEdit.component";

@NgModule({
    imports: [
        groupReportRoutes,
        SharedModule
    ],
    declarations: [
        GroupComponent,
        GroupAddComponent,
        GroupEditComponent,


    ],
    providers: []
})
export class GroupModule { }
