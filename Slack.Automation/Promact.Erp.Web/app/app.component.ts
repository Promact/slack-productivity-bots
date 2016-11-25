
import { Component } from '@angular/core';
import { LoaderService } from './shared/loader.service'
@Component({
    selector: 'my-app',
    templateUrl: './app/index.html'

})
    
export class AppComponent {
    constructor(private loader: LoaderService) { }
}