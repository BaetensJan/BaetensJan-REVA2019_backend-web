import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Router} from "@angular/router";
import {Observable} from "rxjs";
import {TeacherRequest} from "../models/teacherRequest.model";
import {map} from "rxjs/operators";

@Injectable({
  providedIn: 'root'
})
export class QuestionService {
  /**
   * @ignore
   */
  private readonly _url = '/API/Auth';

  /**
   * Constructor
   *
   * @param http
   * @param router
   */
  constructor(private http: HttpClient, private router: Router) {
  }

  /**
   * Makes call to backend and returns all teacherRequests
   */
  sendQuestion(email: string, subject: string, message: string): Observable<boolean> {
    return this.http
      .post(`${this._url}/SendQuestion`, {email, subject, message})
      .pipe(
        map((res: any) => {
          return true;
        })
      );
  }

  /**
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
    return this.http.post(`${this._url}/teacherRequest/sendRequest`, teacherReq).pipe(
      map((res: any) => {
        return true;
      })
    );
  }
}
