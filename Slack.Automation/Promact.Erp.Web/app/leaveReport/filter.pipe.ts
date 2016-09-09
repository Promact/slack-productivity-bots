import { Injectable, Pipe, PipeTransform } from '@angular/core';
import { LeaveReport } from './leaveReport-List/leaveReport-List.model';

@Pipe({ name: 'filter' })

@Injectable()
export class FilterPipe implements PipeTransform {

    transform(leaveReports: LeaveReport[], employeeName: string, utilisedCasualLeave: number, role: string): any {
        if (leaveReports === null) {
            return null;
        }

        if ((utilisedCasualLeave == 0 || utilisedCasualLeave == undefined) && (employeeName === undefined || employeeName === "" || employeeName === null) && (role === undefined || role === "" || role === null)) {
            return leaveReports;
        }

        if ((employeeName === "" || employeeName === undefined) && (role === "" || role === undefined)) {
            return leaveReports.filter(y => y.UtilisedCasualLeave == utilisedCasualLeave);
        }

        if ((employeeName === "" || employeeName === undefined) && utilisedCasualLeave == 0) {
            return leaveReports.filter(y => y.Role.toLowerCase().startsWith(role.toLowerCase()));
        }

        if (utilisedCasualLeave == 0 && (role == "" || role === undefined)) {
            return leaveReports.filter(y => y.EmployeeName.toLowerCase().startsWith(employeeName.toLowerCase()));
        }

        if (utilisedCasualLeave == 0) {
            return leaveReports.filter(y => y.EmployeeName.toLowerCase().startsWith(employeeName.toLowerCase()) || y.Role.toLowerCase().startsWith(role.toLowerCase()));
        }

        if (employeeName === "") {
            return leaveReports.filter(y => y.Role.toLowerCase().startsWith(role.toLowerCase()) || y.UtilisedCasualLeave == utilisedCasualLeave);
        }

        if (role === "") {
            return leaveReports.filter(y => y.EmployeeName.toLowerCase().startsWith(employeeName.toLowerCase()) || y.UtilisedCasualLeave == utilisedCasualLeave);
        }


        return leaveReports.filter(y => y.EmployeeName.toLowerCase().startsWith(employeeName.toLowerCase()) || y.UtilisedCasualLeave == utilisedCasualLeave || y.Role.startsWith(role)) 

    }
}
