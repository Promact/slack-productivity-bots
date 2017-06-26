import { NgModule } from "@angular/core";
import { groupReportRoutes } from "./group.routes";
import { GroupComponent } from "./group.component";
import { GroupAddComponent } from "./GroupAdd/groupAdd.component";
import { GroupEditComponent } from "./GroupEdit/groupEdit.component";
import { GroupListComponent } from "./GroupList/groupList.component";
import { GroupService} from "./group.service";
import { SharedModule } from "../shared/shared.module";
import { MaterialAutoSelectChip } from '../shared/angular-material-chip-autoselect.service';

@NgModule({
    imports: [
        groupReportRoutes,
        SharedModule
    ],
    declarations: [
        GroupComponent,
        GroupAddComponent,
        GroupListComponent,
        GroupEditComponent
    ],
    providers: [GroupService, MaterialAutoSelectChip]
})
export class GroupModule { }
