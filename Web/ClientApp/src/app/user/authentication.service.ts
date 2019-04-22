import {BehaviorSubject} from 'rxjs/BehaviorSubject';
import {Observable} from 'rxjs/Observable';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {catchError, map} from 'rxjs/operators';
import {Router} from '@angular/router';
import {throwError} from "rxjs";

@Injectable()
export class AuthenticationService {
  /**
   * @ignore
   */
  public redirectUrl: string;
  /**
   * @ignore
   */
  private readonly _tokenKey = 'currentUser';
  /**
   * @ignore
   */
  private readonly _url = '/api/auth';
  /**
   * Checks if logged in user is an administrator.
   */
  private _isAdmin$: BehaviorSubject<boolean>;
  /**
   * Checks if logged in user is a superAdministrator.
   */
  private _isSuperAdmin$: BehaviorSubject<boolean>;

  /**
   * Constructor
   *
   * @param http
   * @param router
   */
  constructor(private http: HttpClient, private router: Router) {
    // localStorage.removeItem(this._tokenKey); //TODO if error with can't parse jwt -> remove the key

    let parsedToken = AuthenticationService.parseJwt(localStorage.getItem(this._tokenKey));
    if (parsedToken) {
      const expires =
        new Date(parseInt(parsedToken.exp, 10) * 1000) < new Date();
      if (expires) {
        localStorage.removeItem(this._tokenKey);
        parsedToken = null;
      }
    }
    this._user$ = new BehaviorSubject<string>(
      parsedToken && parsedToken.username
    );
    this._school$ = new BehaviorSubject<string>(
      parsedToken && parsedToken.schoolName
    );

    this.initAdmin(parsedToken);
    this._isSuperAdmin$ = new BehaviorSubject<boolean>(parsedToken && parsedToken.username == "admin");
  }

  private initAdmin(parsedToken: any) {
    this._isAdmin$ = new BehaviorSubject<boolean>(parsedToken && JSON.parse(parsedToken.isAdmin.toLowerCase()));
  }

  /**
   * @ignore
   */
  private _user$: BehaviorSubject<string>;

  /**
   * Getter for user
   */
  get user$(): BehaviorSubject<string> {
    return this._user$;
  }

  private _school$: BehaviorSubject<string>;

  get school$(): BehaviorSubject<string> {
    return this._school$;
  }

  get isModerator$(): BehaviorSubject<boolean> {
    return this._isAdmin$;
  }

  get isSuperAdmin$(): BehaviorSubject<boolean> {
    return this._isSuperAdmin$;
  }

  get isLoggedIn$(): BehaviorSubject<boolean> {
    return new BehaviorSubject<boolean>(this._user$.getValue() != null);
  }

  /**
   * Getter for token
   */
  get token(): string {
    const localToken = localStorage.getItem(this._tokenKey);
    return !!localToken ? localToken : '';
  }

  /**
   * Parser for jwt token when authenticating user
   *
   * @param token
   */
  static parseJwt(token) {
    if (!token) {
      return null;
    }
    try {
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      return JSON.parse(window.atob(base64));
    } catch (err) {
      return null;
    }
  }

  /**
   * Login user with backend call
   *
   * @param username
   * @param password
   */
  login(username: string, password: string): Observable<boolean> {
    return this.http.post(`${this._url}/LoginWebTeacher`, {username, password}).pipe(
      map((res: any) => {
        const token = res.token;
        if (token) {
          const token = res.token;
          let parsedToken = AuthenticationService.parseJwt(token);
          this._isAdmin$ = new BehaviorSubject<boolean>(JSON.parse(parsedToken.isAdmin.toLowerCase()));
          this._isSuperAdmin$ = new BehaviorSubject<boolean>(parsedToken.username == "admin");

          this.setTokenAndInitiateAttributes(res.token, username, parsedToken.schoolName);
          return true;
        }
        return false;
      })
    )
  }

  /**
   * Register user with backend call
   *
   * @param email
   * @param username
   * @param password
   * @param schoolName
   */
  register(email: string, username: string, password: string, schoolName: string): Observable<boolean> {
    return this.http.post(`${this._url}/CreateTeacher`, {email, username, password, schoolName}).pipe(
      map((res: any) => {
        const token = res.token;
        if (token) {
          this.setTokenAndInitiateAttributes(res.token, username, schoolName);
          return true;
        }
        return false;
      })
    );
  }

  /**
   * Removes token to logout user
   */
  logout() {
    if (this.user$.getValue()) {
      localStorage.removeItem(this._tokenKey);

      setTimeout(() => this._user$.next(null));
      setTimeout(() => this._isAdmin$.next(null));
      setTimeout(() => this._isSuperAdmin$.next(null));
      setTimeout(() => this._school$.next(null));
      this.router.navigate(['/']);
    }
  }

  forgotPassword(email: string): Observable<boolean> {
    return this.http.post(`${this._url}/ForgotPassword`, {email}).pipe(
      map((res: any) => {
        return true;
      })
    );
  }

  resetPassword(email: string, code: string, password: string) {
    return this.http.post(`${this._url}/ResetPassword`, {code, email, password}).pipe(
      map((res: any) => {
        return true;
      })
    );
  }

  /**
   * Checks username availability using backend
   *
   * @param username
   */
  checkUserNameAvailability(username: string): Observable<boolean> {
    return this.http.get(`${this._url}/CheckUsername/${username}`).pipe(
      map((item: any) => {
        if (item.username === 'alreadyexists') {
          return false;
        } else {
          return true;
        }
      })
    );
  }

  checkEmailAvailability(email: string): Observable<boolean> {
    return this.http.get(`${this._url}/CheckEmail/${email}`).pipe(
      map((item: any) => {
        if (item.email === 'alreadyexists') {
          return false;
        } else {
          return true;
        }
      })
    );
  }

  changePassword(password: string, newPassword: string): Observable<boolean> {
    return this.http.post(`${this._url}/ChangePassword`, {CurrentPassword: password, NewPassword: newPassword})
      .pipe(
        map((res: any) => {
          return res;
        }),
        catchError(err => throwError(new Error('wrong password')))
      )
  }

  private setTokenAndInitiateAttributes(token, username: string, schoolName: string) {
    localStorage.setItem(this._tokenKey, token);
    this._user$.next(username);
    this._school$.next(schoolName);
  }

  getTourStatus(): Observable<boolean> {
    return this.http.get(`${this._url}/TourStatus`)
      .pipe(
        map((res: any) => {
          return res.isEnabled;
        }),
        catchError(err => throwError(new Error('wrong password')))
      )
  }

  enableTour(): Observable<boolean> {
    return this.http.post(`${this._url}/EnableTour`, {EnableTour: true})
      .pipe(
        map((res: any) => {
          return res;
        }),
        catchError(err => throwError(new Error('wrong value')))
      )
  }

  disableTour(): Observable<boolean> {
    return this.http.post(`${this._url}/DisableTour`, {EnableTour: false})
      .pipe(
        map((res: any) => {
          return res;
        }),
        catchError(err => throwError(new Error('wrong value')))
      )
  }
}
