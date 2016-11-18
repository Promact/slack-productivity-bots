import { TaskMailDetailsModel } from './taskmaildetails.model';
export class TaskMailModel {
    UserName: string;
    UserId: string;
    UserRole: string;
    UserEmail: string;
    CreatedOn: Date;
    CreatedOns: string;
    IsMax: Date;
    IsMin: Date;
    TaskMails: Array<TaskMailDetailsModel>;
}