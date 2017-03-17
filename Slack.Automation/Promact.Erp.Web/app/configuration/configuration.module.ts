import { NgModule } from "@angular/core";
import { SharedModule } from '../shared/shared.module';
import { ConfigurationService } from './configuration.service';
import { ConfigurationComponent } from './configuration.component';
import { configurationRoutes } from './configuration.routes';

@NgModule({
    imports: [
        SharedModule,
        configurationRoutes
    ],
    declarations: [
        ConfigurationComponent
    ],
    providers: [
        ConfigurationService
    ]
})
export class ConfigurationModule { }