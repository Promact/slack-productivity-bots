import { Injectable, Pipe, PipeTransform } from '@angular/core';
import { LeaveReport } from './leaveReport-List/leaveReport-List.model';

@Pipe({ name: 'filter' })

@Injectable()
export class FilterPipe implements PipeTransform {

    transform(leaveReports: LeaveReport[], employeeName: string, utilisedCasualLeave: number): any {
        if (leaveReports == null) {
            return null;
        }

        if ((utilisedCasualLeave == 0 || utilisedCasualLeave == undefined) && (employeeName == undefined || employeeName == "" || employeeName == null)){
            return leaveReports;
        }

        if (employeeName == "") {
            return leaveReports.filter(y => y.UtilisedCasualLeave == utilisedCasualLeave);
             
        }

        return leaveReports.filter(x => x.EmployeeName.toLowerCase().startsWith(employeeName.toLowerCase()) || x.UtilisedCasualLeave == utilisedCasualLeave );
             
    }
}
