import { ModuleWithProviders } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { GroupComponent } from "./group.component";
import { GroupAddComponent } from "./GroupAdd/groupAdd.component";
import { GroupEditComponent } from "./GroupEdit/groupEdit.component";
import { GroupListComponent } from "./GroupList/groupList.component";

const GroupReportRoutes: Routes = [{
    path: "group",
    component: GroupComponent,
    children: [{ path: "", component: GroupListComponent },
    { path: "add", component: GroupAddComponent },
    { path: "edit/:id", component: GroupEditComponent }]
}];
export const groupReportRoutes: ModuleWithProviders = RouterModule.forChild(GroupReportRoutes);
