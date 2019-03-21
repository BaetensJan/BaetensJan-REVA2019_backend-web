import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs/Observable';
import {map} from 'rxjs/operators';
import {School} from "../models/school.model";

@Injectable({
  providedIn: 'root'
})

export class SchoolDataService {
  /**
   * Base url of the to connected connection
   */
  private readonly _appUrl = '/API/School';

  /**
   * Constructor
   *
   * @param http
   */
  constructor(private http: HttpClient) {
  };

  /**
   * Makes call to the backend and returns a school
   *
   * @param schoolId
   */
  getSchool(schoolId: number): Observable<School> {
    return this.http
      .get(`${this._appUrl}/${schoolId}`)
      .pipe(map(School.fromJSON));
  };

  /**
   * Makes call to the backend and returns all schools
   *
   * @param schoolId
   */
  schools(): Observable<School[]> {
    return this.http
      .get(`${this._appUrl}`)
      .pipe(map((list: any[]): School[] => list.map(School.fromJSON)));
  };

  /**
   * Update schools loginName
   *
   * @param schoolId
   * @param loginName
   */
  updateSchoolLoginName(schoolId: number, loginName: string): Observable<School> {
    return this.http.put(`${this._appUrl}/UpdateSchoolLoginName/${schoolId}`, {schoolLoginName: loginName}).pipe(
      map((item: School) => {
        return School.fromJSON(item);
      })
    )
  };

  /**
   * Checks schoolname availability using backend
   *
   * @param schoolName
   */
  checkSchoolNameAvailability(schoolName: string): Observable<boolean> {
    return this.http.get(`${this._appUrl}/CheckSchoolName/${schoolName}`).pipe(
      map((item: any) => {
        if (item.schoolName === 'alreadyexists') {
          return false;
        } else {
          return true;
        }
      })
    );
  };

  /**
   * Checks school loginName availability using backend
   *
   * @param loginName
   */
  checkLoginNameAvailability(loginName: string): Observable<boolean> {
    return this.http.get(`${this._appUrl}/CheckLoginName/${loginName}`).pipe(
      map((item: any) => {
        if (item.loginName === 'alreadyexists') {
          return false;
        } else {
          return true;
        }
      })
    );
  };
}
