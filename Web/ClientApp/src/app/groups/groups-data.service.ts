import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs/Observable';
import {map} from 'rxjs/operators';
import {Group} from "../models/group.model";
import {School} from "../models/school.model";
import {pipe} from "rxjs";

@Injectable({
  providedIn: 'root'
})

export class GroupsDataService {
  /**
   * Base url of the to connected connection
   */
  private readonly _appUrl = '/API/Group';

  /**
   * Constructor
   *
   * @param http
   */
  constructor(private http: HttpClient) {
  }

  /**
   * Getter for Groups with only the members attribute (without assignments, exhibitor,...)
   */
  get allGroups(): Observable<Group[]> {
    return this.http
      .get(`${this._appUrl}/Groups/`)
      .pipe(map((list: any[]): Group[] => list.map(Group.fromJSON)));
  }

  /**
   * Getter for Groups with the id of the school
   */
  groupsBasicById(id): Observable<Group[]> {
    return this.http
      .get(`${this._appUrl}/GroupsBasicBySchoolId/${id}`)
      .pipe(map((list: any[]): Group[] => list.map(Group.fromJSON)));
  }


  /**
   * Getter for Groups
   */
  get groups(): Observable<Group[]> {
    return this.http
      .get(`${this._appUrl}/Groups/`)
      .pipe(map((list: any[]): Group[] => list.map(Group.fromJSON)));
  }

  /**
   * Getter for Groups with the id of the school
   */
  groupsBySchoolId(schoolId): Observable<Group[]> {
    return this.http
      .get(`${this._appUrl}/Groups/${schoolId}`)
      .pipe(map((list: any[]): Group[] => list.map(Group.fromJSON)));
  }

  // /**
  //  * Makes call to backend and returns Schools
  //  */
  // get school(): Observable<School[]> {
  //   return this.http
  //     .get(`${this._appUrl}/Groups`)
  //     .pipe(map((list: any[]): School[] => list.map(School.fromJSON)));
  // }

  /**
   * Makes call to the backend and adds new Group
   *
   * @param group
   * @param schoolId
   */
  addNewGroup(schoolId, group): Observable<Group> {
    return this.http
      .post(`${this._appUrl}/CreateAndReturnGroup/${schoolId}`, group)
      .pipe(map(Group.fromJSON));
  }

  updateUser(schoolId, group): Observable<Group> {
    return this.http
      .put(`${this._appUrl}/UpdateUser/${schoolId}`, group)
      .pipe(map(Group.fromJSON));
  }

  /**
   * Makes call to the backend and updates Group
   *
   * @param group
   */
  updateGroup(group): Observable<Group> {
    return this.http
      .put(`${this._appUrl}/UpdateGroup/${group.groupId}`, group)
      .pipe(map(Group.fromJSON));
  }

  /**
   * Makes call to the backend and removes a group
   * @param rec
   */
  removeGroup(rec: Group): Observable<Group> {
    return this.http
      .delete(`${this._appUrl}/deleteGroup/${rec.id}`)
      .pipe(map(Group.fromJSON));
  }

  /**
   * Makes call to the backend and returns a group
   *
   * @param id
   */
  getGroup(id: number): Observable<Group> {
    return this.http
      .get(`${this._appUrl}/Group/${id}`)
      .pipe(map(Group.fromJSON));
  }

  /**
   * Makes call to the backend and add a member to a group.
   *
   * @param groupId: id of the group to which we will add a member to.
   * @param memberName: name of a new member of the group
   */
  addMember(groupId: number, memberName: string): Observable<Group> {
    return this.http
      .get(`${this._appUrl}/addMember/${groupId}/${memberName}`)
      .pipe(map(Group.fromJSON));
  }

  /**
   * Makes call to the backend and add a member to a group.
   *
   * @param groupId: id of the group to which we will add a member to.
   * @param memberName: name of a new member of the group
   */
  removeMember(groupId: number, memberName: string): Observable<Group> {
    return this.http
      .get(`${this._appUrl}/removeMember/${groupId}/${memberName}`)
      .pipe(map(Group.fromJSON));
  }

  /**
   * Checks groupname availability using backend
   *
   * @param schoolId
   * @param groupName
   */
  checkGroupNameAvailability(/*schoolId: string, */groupName: string): Observable<boolean> {
    return this.http.get(`${this._appUrl}/CheckGroupName/${groupName}`).pipe(
      map((item: any) => {
        if (item.groupName === 'alreadyexists') {
          return false;
        } else {
          return true;
        }
      })
    );
  }
}
