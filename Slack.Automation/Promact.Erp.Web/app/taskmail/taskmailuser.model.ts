import { taskmailModel } from './taskmail.model';
export class taskmailuserModel {
    UserName: string;
    UserId: string;
    UserRole: string;
    UserEmail: string;
    CreatedOn: Date;
    CreatedOns: string;
    IsMax: Date;
    IsMin: Date;
    TaskMails: Array<taskmailModel>;
}