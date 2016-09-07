import {MockBackend, MockConnection} from "@angular/http/testing";
import {ReflectiveInjector, bind} from "@angular/core";
import {Http, HTTP_BINDINGS, XHRBackend} from "@angular/http";
import { Injectable } from '@angular/core';

@Injectable()
export class TestConnection {
    mockConnection(url: string) {
        let connection: MockConnection;
        let injector = ReflectiveInjector.resolveAndCreate([
            HTTP_BINDINGS,
            MockBackend,
            bind(XHRBackend).toAlias(MockBackend)
        ]);

        let backend = injector.get(MockBackend);
        let http = injector.get(Http);
        //Assign any newly-created connection to local variable
        backend.connections.subscribe((c: MockConnection) => {
            connection = c;
        });
        http.request(url);
        return connection;
    }
}
