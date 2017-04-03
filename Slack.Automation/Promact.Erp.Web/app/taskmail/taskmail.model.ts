import { TaskMailDetailsModel } from './taskmaildetails.model';
export class TaskMailModel {
    UserName: string;
    UserId: string;
    UserRole: string;
    UserEmail: string;
    CreatedOn: Date;
    CreatedOns: string;
    MaxDate: Date;
    MinDate: Date;
    TaskMails: Array<TaskMailDetailsModel>;
}