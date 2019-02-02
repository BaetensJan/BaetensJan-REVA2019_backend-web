import { Injectable } from '@angular/core';
import {map} from 'rxjs/operators';
import {Observable} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {Router} from "@angular/router";

@Injectable({
  providedIn: 'root'
})
export class InvitationService {
  /**
   * @ignore
   */
  private readonly _url = '/api/invitation';

  /**
   * Constructor
   *
   * @param http
   * @param router
   */
  constructor(private http: HttpClient, private router: Router) { }

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
    return this.http.post(`${this._url}/inviteRequest`, {email, name, surname, schoolName, note}).pipe(
      map((res: any) => {
       return true;
      })
    );
  }

  /**
   * Create Teacher and send email to Teacher
   *
   * @param email
   * @param name
   * @param surname
   * @param note
   * @param schoolName
   */
  createTeacher(email: string, name: string, surname: string, schoolName: string): Observable<boolean> {
    return this.http.post(`${this._url}/inviteRequest`, {email, name, surname, schoolName}).pipe(
      map((res: any) => {
       return true;
      })
    );
  }
}
