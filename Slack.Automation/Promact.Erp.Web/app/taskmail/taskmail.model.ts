import { TaskMailStatus } from '../enums/TaskMailStatus';
export class taskmailModel {
    Id: number;
    UserName: string;
    CreatedOn: Date;
    CreatedOns: string;
    Description: string;
    Comment: string;
    Hours: number;
    TotalItems: number;
    Status: TaskMailStatus;
    StatusName: string;
    
}