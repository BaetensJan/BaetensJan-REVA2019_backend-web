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
}
