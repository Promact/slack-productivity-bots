import { Project } from './project.model';
export class MailSetting {
    Id: number;
    Module: string;
    Project: Project;    
    To: Array<string>;
    CC: Array<string>;
    SendMail: boolean;
}