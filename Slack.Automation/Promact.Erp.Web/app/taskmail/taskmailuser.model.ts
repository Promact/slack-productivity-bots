import { taskmailModel } from './taskmail.model';
export class taskmailuserModel {
    UserName: string;
    UserId: string;
    UserRole: string;
    UserEmail: string;
    CreatedOn: Date;
    CreatedOns: string;
    isMax: Date;
    isMin: Date;
    TaskMails: Array<taskmailModel>;
}