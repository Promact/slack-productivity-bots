import {  Pipe, PipeTransform } from '@angular/core';
import { LeaveReport } from './leaveReport-List/leaveReport-List.model';

@Pipe({ name: 'sgFilter' })

export class FilterPipe implements PipeTransform {
    transform(leaveReports: LeaveReport[], employeeName: string, role: string)  {
        if (employeeName === undefined || employeeName === "" && (role === undefined || role === "" || role === null)) {
            return leaveReports;
        }
        if (role === undefined || role === "") {
            return leaveReports.filter(x => x.EmployeeName.toLowerCase().startsWith(employeeName.toLowerCase()));
        }
        if (employeeName === undefined || employeeName === "") {
            return leaveReports.filter(y => y.Role.toLowerCase().startsWith(role.toLowerCase()));
        }
        else {
            return leaveReports.filter(y => y.EmployeeName.toLowerCase().startsWith(employeeName.toLowerCase()) && y.Role.toLowerCase().startsWith(role.toLowerCase()));
        }
    }
}
