import { ModuleWithProviders } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { ConfigurationComponent } from './configuration.component';

const ConfigurationRoutes: Routes = [{
    path: "configuration",
    component: ConfigurationComponent
}];
export const configurationRoutes: ModuleWithProviders = RouterModule.forChild(ConfigurationRoutes);