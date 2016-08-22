//import { bootstrap }      from '@angular/platform-browser-dynamic';
//import { HTTP_PROVIDERS } from '@angular/http';

//import { AppComponent }   from './app.component';
//import { appRouterProviders } from './app.routes';

//bootstrap(AppComponent, [appRouterProviders, HTTP_PROVIDERS]);



import { NgModule, enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { AppModule } from './app.module';

enableProdMode();
platformBrowserDynamic().bootstrapModule(AppModule);