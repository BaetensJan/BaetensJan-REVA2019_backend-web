import {AuthenticationService} from './authentication.service';
import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot} from '@angular/router';

@Injectable()
export class AuthGuardService implements CanActivate {

  constructor(private _authService: AuthenticationService, private router: Router) {
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    console.debug(state.url);
    console.debug(`Logged in: ${this._authService.isLoggedIn$.getValue()}`);
    console.debug(`Is Admin: ${this._authService.isModerator$.getValue()}`);


    // user is logged in.
    if (this._authService.isLoggedIn$.getValue()) {

      let pages =
        [
          '/group/groups', "/group/updateGroup",
          "/wachtwoord-veranderen", "/opdrachten",
          "/beursplan",
          "/logout",
        ];

      if (pages.includes(state.url)
        || AuthGuardService.isAssignmentDetail(state.url)) {
        return this.youShallXXXPass(true, state.url);
      } else {

        // check if user is admin.
        if (this._authService.isModerator$.getValue()) {

          let adminPages =
            [
              "/categorieen", "/categorie",
              "/exposanten", "/exposant",
              "/vragen", "/vraag", "/upload-csv",
              "/requests", "/invite-request", "/accept-request",
              "/enable-tour"
            ];

          if (adminPages.includes(state.url)
            || AuthGuardService.isAssignmentDetail(state.url)
            || AuthGuardService.isInviteRequest(state.url)
            || AuthGuardService.isAcceptRequest(state.url)) {
            return this.youShallXXXPass(true, state.url);
          }
        }

        return this.youShallXXXPass(false, "/");
      }
    }

    // user is not logged in.
    else {

      let pages =
        ['/login',
          "/forgot-password", "/wachtwoord-vergeten-confirmation", "/reset-wachtwoord",
          "/invite-request",
          '/home', '/'
        ];

      if (pages.includes(state.url)) return this.youShallXXXPass(true, state.url);
      else return this.youShallXXXPass(false, "/login");
    }
  }

  private youShallXXXPass(pass: boolean, url) {
    if (pass) {
      this._authService.redirectUrl = url;
      return true;
    }
    this.router.navigate([url]);
    return false;
  }

  /**
   * Method used to check if going to assignmentdetail AND has queryParams (groupId),
   * e.g. /assignmentdetail?groupId=3
   *
   * @param url
   */
  private static isAssignmentDetail(url): boolean {
    let detailString = "/assignmentdetail?groupId=";
    if (url.startsWith(detailString)) {
      let ret = url.replace(detailString, '');

      // check if everything after detailString is a number (groupId)
      if (!isNaN(Number(ret))) return true;
    }
  }

  private static isAcceptRequest(url): boolean {
    let detailString = "/accept-request?requestId=";
    if (url.startsWith(detailString)) {
      let ret = url.replace(detailString, '');

      // check if everything after detailString is a number (requestId)
      if (!isNaN(Number(ret))) return true;
    }
  }

  private static isInviteRequest(url): boolean {
    let detailString = "/invite-request?requestId=";
    if (url.startsWith(detailString)) {
      let ret = url.replace(detailString, '');

      // check if everything after detailString is a number (requestId)
      if (!isNaN(Number(ret))) return true;
    }
  }
}
