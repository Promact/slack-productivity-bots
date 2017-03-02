import { Injectable } from '@angular/core';
import { ResponseOptions, Response } from "@angular/http";
import { GroupModel } from '../../Group/group.model';
import { StringConstant } from '../../shared/stringConstant';

let stringConstant = new StringConstant();

@Injectable()
export class MockGroupService {
    //private UserUrl = 'api/user';
    constructor() { }

    /*This service used for get group list*
    *
    */
    getListOfGroup() {
        let mockGroup = new GroupModel();
        var emails = stringConstant.emails;
        mockGroup.Emails = emails;
        mockGroup.Name = stringConstant.testGroupName;
        mockGroup.Type = 2;
        return Promise.resolve(mockGroup);
    }

    /*This service used for add new group*
     * 
     * @param groupModel
     */
    addGroup(groupModel: GroupModel) {
        return Promise.resolve(groupModel.Name);
    }

    /*This service used for get group by id*
     * 
     * @param id
     */
    getGroupbyId(id: number) {
        let mockGroup = new MockGroup(id);
        if (id === 1) {
            var emails = stringConstant.emails;
            mockGroup.Emails = emails;
            mockGroup.Name = stringConstant.testGroupName;
            mockGroup.Type = 2;
        }
        return Promise.resolve(mockGroup);
    }

    /*This service used for update group*
     * 
     * @param groupModel
     */
    updateGroup(groupModel: GroupModel) {
        return Promise.resolve(groupModel);
    }

    /*This service used for check group name is already exists or not*
     * 
     * @param groupName
     * @param id
     */
    checkGroupNameIsExists(name: string, id: number) {
        if (name === stringConstant.testGroupName)
            return Promise.resolve(true);
        else
            return Promise.resolve(false);
    }

    /*This service used for delete group by id.*
     * 
     * @param id
     */
    deleteGroupById(id: number) {
        return Promise.resolve(true);
    }

}

class MockGroup extends GroupModel {
    constructor(id: number) {
        super();
        this.Id = id;
    }

}