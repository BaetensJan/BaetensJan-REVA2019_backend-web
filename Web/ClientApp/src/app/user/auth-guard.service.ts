import {AuthenticationService} from './authentication.service';
import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot} from '@angular/router';

@Injectable()
export class AuthGuardService implements CanActivate {

  constructor(private authService: AuthenticationService, private router: Router) {
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    console.debug(state.url);
    console.debug(`Admin Required: ${this.isAdminRequired(state.url)}`);
    console.debug(`Login Required: ${this.isLoggedInPage(state.url)}`);
    console.debug(`NonLogin Required: ${this.isNonLoggedInPage(state.url)}`);
    console.debug(`Logged in: ${this.authService.isLoggedIn$.getValue()}`);
    console.debug(`Is Admin: ${this.authService.isModerator$.getValue()}`);

    //todo beter om eerst rol te bekijken van user (admin, loggedin, notlogged in en vervolgens pas url te checken)
    if (state.url == "/invite-request") {
      return !(this.authService.isLoggedIn$.getValue()
        && this.authService.isModerator$.getValue() == false);
    }
    if (this.isAdminRequired(state.url)) {
      if (this.authService.isLoggedIn$.getValue()) {
        if (this.authService.isModerator$.getValue()) {
          return true;
        } else {
          //unauthorized => redirect to unauthorized
          this.router.navigate(['/login']);
          return false;
        }
      }
      this.authService.redirectUrl = state.url;
      this.router.navigate(['/login']);
      return false;
    }
    if (this.isLoggedInPage(state.url)) {
      if (this.authService.isLoggedIn$.getValue()) {
        return true
      }
      this.authService.redirectUrl = state.url;
      this.router.navigate(['/login']);
      return false;
    }
    if (this.isNonLoggedInPage(state.url)) {
      if (this.authService.isLoggedIn$.getValue()) {
        this.router.navigate(['/home']);
        return false;
      }
      return true;
    } else
      this.router.navigate(['/home']); // unauthorized.
    return false;
  }


  private isLoggedInPage(url): boolean {
    // assignmentsdetail has queryParams (groupId), e.g. /assignmentdetail?groupId=3
    let detailString = "/assignmentdetail?groupId=";
    if (url.startsWith(detailString)) {
      let ret = url.replace(detailString, '');

      // check if everything after detailString is a number (groupId)
      if (!isNaN(Number(ret))) return true;
    }

    let pages = ['/group/groups', "/opdrachten", "/logout", "/group/updateGroup", "/wachtwoord-veranderen"];
    return pages.includes(url);
  }

  private isNonLoggedInPage(url): boolean {
    //Pages logged in users aren't allowed to access anymore
    let pages = ['/login', "/invite-request", "/register", "wachtwoord-vergeten", "/wachtwoord-vergeten-confirmation", "/reset-wachtwoord"];
    return pages.includes(url);

  }

  private isAdminRequired(url): boolean {
    let detailString = "/invite-request?requestId=";
    if (url.startsWith(detailString)) {
      let ret = url.replace(detailString, '');

      // check if everything after detailString is a number (requestId)
      if (!isNaN(Number(ret))) return true;
    }

    let adminPages = ["/categorieen", "/categorie", "/exposanten", "/exposant",
      "/beursplan", "/requests", "/vragen", "/vraag", "/upload-csv"];
    return adminPages.includes(url);
  }
}
