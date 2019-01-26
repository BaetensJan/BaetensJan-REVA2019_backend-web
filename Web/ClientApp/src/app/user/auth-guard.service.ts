import { AuthenticationService } from './authentication.service';
import { Injectable } from '@angular/core';
import { Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { CanActivate } from '@angular/router';

@Injectable()
export class AuthGuardService implements CanActivate {

  constructor(private authService: AuthenticationService, private router: Router) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    let stateUrl = state.url;
    console.log(stateUrl);
    // user wants to go to the 'administration pages'.
    if (stateUrl === "/categorieen" || stateUrl === "/exposanten") {
      if (this.authService.user$.getValue() && this.authService.isModerator$.getValue()) {
        console.log("permission granted");
        return true;
      }
      this.authService.redirectUrl = "";
      this.router.navigate(['/']); // go to home page if access denied.
      console.log("permission denied");
      return false;
      return true;
    } else { // user wants to go to the 'login pages'.
      if (this.authService.user$.getValue()) {
        console.log("permission granted");
        return true;
      }
      this.authService.redirectUrl = state.url;
      this.router.navigate(['/login']);
      return false;
    }
  }
}
