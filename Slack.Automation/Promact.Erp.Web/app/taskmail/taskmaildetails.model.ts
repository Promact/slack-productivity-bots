import { TaskMailStatus } from '../enums/TaskMailStatus';
export class TaskMailDetailsModel {
    Id: number;
    CreatedOn: Date;
    CreatedOns: string;
    Description: string;
    Comment: string;
    Hours: number;
    TotalItems: number;
    Status: TaskMailStatus;
    StatusName: string;
   
}