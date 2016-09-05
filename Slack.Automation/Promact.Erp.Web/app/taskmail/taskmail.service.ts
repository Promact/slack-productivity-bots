import { Injectable } from '@angular/core';
import {HttpService} from "../http.service";
import 'rxjs/add/operator/toPromise';
import { taskmailModel } from './taskmail.model';
@Injectable()
export class TaskService {
    private TaskMailUrl = 'api/TaskReport';  // URL to web api
    constructor(private httpService: HttpService<taskmailModel>) { }
    
    getTaskMailReport() {
        return this.httpService.get("taskMailReport");
    }

    getTaskMailDetailsReport(id: number) {
        return this.httpService.get("taskMailDetailsReport/"+ id);
    }
    
    
}
