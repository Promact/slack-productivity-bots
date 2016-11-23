import { taskmailModel } from './taskmail.model';
export class taskmailuserModel {
    UserName: string;
    UserId: string;
    UserRole: string;
    UserEmail: string;
    CreatedOn: Date;
    CreatedOns: string;
    MaxDate: Date;
    MinDate: Date;
    TaskMails: Array<taskmailModel>;
}