//import { bootstrap }      from '@angular/platform-browser-dynamic';
//import { HTTP_PROVIDERS } from '@angular/http';
"use strict";
//import { AppComponent }   from './app.component';
//import { appRouterProviders } from './app.routes';
//bootstrap(AppComponent, [appRouterProviders, HTTP_PROVIDERS]);
var core_1 = require('@angular/core');
var platform_browser_dynamic_1 = require('@angular/platform-browser-dynamic');
var app_module_1 = require('./app.module');
core_1.enableProdMode();
platform_browser_dynamic_1.platformBrowserDynamic().bootstrapModule(app_module_1.AppModule);
//# sourceMappingURL=main.js.map