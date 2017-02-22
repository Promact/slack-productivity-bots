export class MailSetting {
    Id: number;
    Module: string;
    ProjectId: number;
    ProjectName : string
    To: Array<string>;
    CC: Array<string>;
    SendMail: boolean;
}