import { Injectable, Pipe, PipeTransform } from '@angular/core';
import { taskmailModel } from './taskmail.model';

@Pipe({ name: 'filter' })

@Injectable()
export class FilterPipe implements PipeTransform {

    transform(taskmailModels: Array<taskmailModel>, UserName: string, CreatedOn: Date): any {
        if (taskmailModels == null) {
            return null;
        }

        //if ((CreatedOn == undefined) && (UserName == undefined || UserName == "" || UserName == null)) {
        //    return taskmailModels;
        //}

        
        //if ((CreatedOn != undefined) && (UserName == undefined || UserName == "" || UserName == null) ) {
        //    return taskmailModels.filter(y => y.CreatedOns == CreatedOn.toString());
        //}
        //if ((UserName != "" || UserName != undefined || UserName != null) && (CreatedOn == undefined))
        //{
        //    return taskmailModels.filter(x => x.UserName.toLowerCase().startsWith(UserName.toLowerCase()));
        //}
        //return taskmailModels.filter(x => x.UserName.toLowerCase().startsWith(UserName.toLowerCase()) && x.CreatedOns == CreatedOn.toString());

    }
}
