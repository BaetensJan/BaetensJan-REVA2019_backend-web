import { Injectable } from '@angular/core';
import {Group} from "../models/group.model";

@Injectable({
  providedIn: 'root'
})
export class GroupSharedService {

  private _group: Group;
  private _edit: boolean;
  private _schoolId;

  constructor() { }

  get group() {
    return this._group;
  }

  get edit() {
    return this._edit;
  }

  get schoolId() {
    return this._schoolId;
  }

  updateGroup(schoolId, group: Group){
    this._schoolId = schoolId;
    this._edit = true;
    this._group = group;
  }
}
