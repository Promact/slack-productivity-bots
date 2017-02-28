import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from "@angular/http";
import { Observable } from 'rxjs/Rx';
import { StringConstant } from '../shared/stringConstant';
import { GroupModel } from './group.model';

@Injectable()
export class GroupService {
    private headers = new Headers({ 'Content-Type': 'application/json' });
    constructor(private http: Http, private stringConstant: StringConstant) {

    }

    /*This service used for get group list*
   *
   */
    getListOfGroup(): Promise<GroupModel[]> {
        return this.http.get(this.stringConstant.groupUrl).map(res => res.json())
            .toPromise();
    }

    /*This service used for add new group*
     * 
     * @param groupModel
     */
    addGroup(groupModel: GroupModel) {
        return this.http.post(this.stringConstant.groupUrl, JSON.stringify(groupModel), { headers: this.headers })
            .toPromise();
    }

    /*This service used for get group by id*
     * 
     * @param id
     */
    getGroupbyId(id: number) {
        return this.http.get(this.stringConstant.groupUrl + '/' + id).map(res => res.json())
            .toPromise();
    }

    /*This service used for update group*
     * 
     * @param groupModel
     */
    updateGroup(groupModel: GroupModel) {
        return this.http.put(this.stringConstant.groupUrl + '/' + groupModel.Id, JSON.stringify(groupModel), { headers: this.headers })
            .toPromise();

    }

    /*This service used for check group name is already exists or not*
     * 
     * @param groupName
     * @param id
     */
    checkGroupNameIsExists(name: string, id: number) {
        return this.http.get(this.stringConstant.groupUrl + '/available/' + name + '/' + id).map(res => res.json())
            .toPromise();
    }

}

