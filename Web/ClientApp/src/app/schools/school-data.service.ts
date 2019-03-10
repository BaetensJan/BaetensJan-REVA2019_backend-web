import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs/Observable';
import {map} from 'rxjs/operators';
import {School} from "../models/school.model";
import {Group} from "../models/group.model";

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
  }

  /**
   * Makes call to the backend and returns a school
   *
   * @param schoolId
   */
  getSchool(schoolId: number): Observable<School> {
    return this.http
      .get(`${this._appUrl}/${schoolId}`)
      .pipe(map(School.fromJSON));
  }

  /**
   * Makes call to the backend and returns all schools
   *
   * @param schoolId
   */
  schools(): Observable<School[]> {
    return this.http
      .get(`${this._appUrl}`)
      .pipe(map((list: any[]): School[] => list.map(School.fromJSON)));
  }

  /**
   * Checks schoolname availability using backend
   *
   * @param schoolName
   */
  checkSchoolNameAvailability(schoolName: string): Observable<boolean> {
    return this.http.get(`/API/Auth/CheckSchool/${schoolName}`).pipe(
      map((item: any) => {
        if (item.school === 'alreadyexists') {
          return false;
        } else {
          return true;
        }
      })
    )
  }
}
