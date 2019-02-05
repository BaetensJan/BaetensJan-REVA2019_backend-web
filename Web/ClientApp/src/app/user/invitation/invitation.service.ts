import {Injectable} from '@angular/core';
import {map} from 'rxjs/operators';
import {Observable} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {Router} from "@angular/router";
import {Exhibitor} from "../../models/exhibitor.model";
import {TeacherRequest} from "../../models/teacherRequest.model";

@Injectable({
  providedIn: 'root'
})
export class InvitationService {
  /**
   * @ignore
   */
  private readonly _url = '/api/auth';

  /**
   * Constructor
   *
   * @param http
   * @param router
   */
  constructor(private http: HttpClient, private router: Router) {
  }

  /** TODO: create a temporary account, so that Freddy doesn't have to re-type (exclude typo's) all the info.
   * Invite Request to join the web platform
   *
   * @param email
   * @param name
   * @param surname
   * @param note
   * @param schoolName
   */
  inviteRequest(email: string, name: string, surname: string, schoolName: string, note: string): Observable<boolean> {
    console.log({email, name, surname, schoolName, note});
    let teacherReq = new TeacherRequest(name, surname, email, schoolName, note);
    return this.http.post(`/api/teacherRequest/sendRequest`, teacherReq).pipe(
      map((res: any) => {
        return true;
      })
    );
  }

  /**
   * Create Teacher and send email to Teacher
   *
   * @param teacherRequestId: number (the id of the by the admin selected teacherRequest).
   */
  createTeacher(teacherRequestId: number): Observable<boolean> {
    return this.http.get(`${this._url}/CreateTeacher/${teacherRequestId}`).pipe(
      map((res: any) => {
        return true;
      })
    );
  }

  /**
   * Makes call to backend and returns all teacherRequests
   */
  get teacherRequests(): Observable<TeacherRequest[]> {
    return this.http
      .get(`/api/teacherRequest/requests/`)
      .pipe(map((list: any[]): TeacherRequest[] => list.map(TeacherRequest.fromJSON)));
  }
}
