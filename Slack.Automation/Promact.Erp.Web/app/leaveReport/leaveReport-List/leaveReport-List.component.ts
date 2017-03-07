import { Component, OnInit } from '@angular/core';
import { LeaveReport } from './leaveReport-List.model';
import { LeaveReportService } from '../leaveReport.service';
import { Router } from '@angular/router';
import { StringConstant } from '../../shared/stringConstant';
import { LoaderService } from '../../shared/loader.service';
import { JSPDF } from '../../shared/json.to.pdf';

@Component({
    templateUrl: './app/leaveReport/leaveReport-List/leaveReport-List.html',
})

export class LeaveReportListComponent implements OnInit {
    leaveReports: LeaveReport[] = [];
    errorMessage: string;
    private EmployeeName: string;
    private Role: string;
    noLeaves: string;

    constructor(private leaveReportService: LeaveReportService, private router: Router, private stringConstant: StringConstant, private loader: LoaderService, private jsPDF: JSPDF) { }

    ngOnInit() {
        this.getLeaveReports();
    }

    getLeaveReports() {
        this.loader.loader = true;
        this.leaveReportService.getLeaveReports()
            .subscribe(
            leaveReports => {
                this.leaveReports = leaveReports;
                if (leaveReports.length !== 0) {
                    this.loader.loader = false;
                    return leaveReports;
                }
                else {
                    this.noLeaves = this.stringConstant.noLeaves;
                    this.loader.loader = false;
                    return this.noLeaves;
                }

            },
            error => this.errorMessage = <string>error
            );
    }

    exportDataToPdf() {
        let columns = this.stringConstant.listColumns;
        let rows = [];
        for (let key in this.leaveReports) {
            rows.push([
                this.leaveReports[key].EmployeeName,
                this.leaveReports[key].EmployeeUserName,
                this.leaveReports[key].Role,
                this.leaveReports[key].TotalSickLeave,
                this.leaveReports[key].TotalCasualLeave,
                this.leaveReports[key].UtilisedCasualLeave,
                this.leaveReports[key].BalanceCasualLeave,
                this.leaveReports[key].UtilisedSickLeave,
                this.leaveReports[key].BalanceSickLeave
            ]);
        };
        this.jsPDF.exportJsonToPdf(columns, rows);
    }
}