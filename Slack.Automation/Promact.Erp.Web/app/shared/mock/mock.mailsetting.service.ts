import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { MailSettingAC } from '../MailSetting/mailsettingac.model';
import { MailSetting } from '../MailSetting/mailsetting.model';
import { Project } from '../MailSetting/project.model';

@Injectable()

export class MockMailSettingService {

    addMailSetting(mailSetting: MailSettingAC) {
        return new BehaviorSubject(mailSetting).asObservable();
    }

    getProjectByIdAndModule(id: number, module: string) {
        let mailSetting: MailSetting = new MailSetting;
        mailSetting.Id = 1;
        return new BehaviorSubject(mailSetting).asObservable();
    }

    getAllProjects() {
        let listOfProject: Array<Project> = new Array<Project>();
        let project: Project = new Project;
        project.Id = 1;
        project.Name = "something";
        listOfProject.push(project);
        return new BehaviorSubject(listOfProject).asObservable();
    }

    updateMailSetting(mailSetting: MailSettingAC) {
        return new BehaviorSubject(mailSetting).asObservable();
    }

    getListOfGroups() {
        let groupList: Array<string> = new Array<string>();
        groupList.push("hello");
        return new BehaviorSubject(groupList).asObservable();
    }
}