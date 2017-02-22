
import { Component, OnInit } from '@angular/core';
import { LoaderService } from './shared/loader.service';
import { AppComponentService } from './appcomponent.service';

@Component({
    selector: 'my-app',
    templateUrl: './app/index.html'

})

export class AppComponent implements OnInit {
    userIsAdmin = false;
    constructor(private loader: LoaderService, private httpService: AppComponentService) { }

    ngOnInit() {
        this.httpService.getUserIsAdminOrNot().then((result) => {
            this.userIsAdmin = (result === 'true');
        },
            err => {
                this.userIsAdmin = false;
      });
    }
}