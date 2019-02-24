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
    } else {
      if (state.url == "/invite-request") {
        return true;
      } else {
        this.router.navigate(['/home']); // unauthorized.
        return false;
      }
    }
  }

  private isLoggedInPage(url): boolean {
    console.log(url);
    // assignmentsdetail has queryParams (groupId), e.g. /assignmentdetail?groupId=3
    let detailString = "/assignmentdetail?groupId=";
    if (url.startsWith(detailString)) {
      let ret = url.replace(detailString, '');

      // check if everything after detailString is a number (groupId)
      if (!isNaN(Number(ret))) return true;
    }

    let pages = ['/group/groups', "/opdrachten", "/logout", "/group/updateGroup"];
    return pages.includes(url);
  }

  private isNonLoggedInPage(url): boolean {
    //Pages logged-in-users aren't allowed to access anymore
    let pages = ['/login', "/register"];
    return pages.includes(url);

  }

  private isAdminRequired(url): boolean {
    let adminPages = ["/categorieen", "/categorie", "/exposanten", "/exposant",
      "/beursplan", "/aanvragen", "/vragen", "/vraag", "/upload-csv"];
    return adminPages.includes(url);

  }
}
