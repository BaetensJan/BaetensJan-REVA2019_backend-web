import {BehaviorSubject} from 'rxjs/BehaviorSubject';
import {Observable} from 'rxjs/Observable';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {map} from 'rxjs/operators';
import {Router} from '@angular/router';

/**
 * Parser for jwt token when authenticating user
 *
 * @param token
 */
function parseJwt(token) {
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
   * Constructor
   *
   * @param http
   * @param router
   */
  constructor(private http: HttpClient, private router: Router) {
    // localStorage.removeItem(this._tokenKey); //TODO if error with can't parse jwt -> remove the key

    let parsedToken = parseJwt(localStorage.getItem(this._tokenKey));
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

  get isModerator$(): BehaviorSubject<boolean> {
    return this._isAdmin$;
  }

  get isLoggedIn(): boolean {
    return this._user$.getValue() != null;
  }


  /**
   * Getter for token
   */
  get token(): string {
    const localToken = localStorage.getItem(this._tokenKey);
    return !!localToken ? localToken : '';
  }

  /**
   * Login user with backend call
   *
   * @param username
   * @param password
   */
  login(username: string, password: string): Observable<boolean> {
    //TODO check if token already exist (then api call to backend is unnecessary) => if (localStorage.getItem(this._tokenKey)*/
    return this.http.post(`${this._url}/LoginWebTeacher`, {username, password}).pipe(
      map((res: any) => {
        const token = res.token;
        if (token) {
          const token = res.token;
          let parsedToken = parseJwt(token);
          if (this.checkUserRole(parsedToken)) { // check if user is a teacher
            this.setTokenAndUsername(res.token, username);
            return true;
          }
          return false; // checkUserRole failed => user is a group
        } else return false;
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
          this.setTokenAndUsername(res.token, username);
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

  private setTokenAndUsername(token, username) {
    localStorage.setItem(this._tokenKey, token);
    this._user$.next(username);
  }

  private checkUserRole(parsedToken): boolean {
    // check if user is administrator.
    this._isAdmin$ = new BehaviorSubject<boolean>(JSON.parse(parsedToken.isAdmin.toLowerCase()));

    // check if user is a group. Groups have no access to web.
    return !(parsedToken && parsedToken.group);
  }

  changePassword(password: string): Observable<boolean> {
    return this.http.post(`${this._url}/ChangePassword`, {password}).pipe(
      map((res: any) => {
        return true;
      })
    );
  }
}
