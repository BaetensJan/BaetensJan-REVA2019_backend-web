import {Component} from '@angular/core';
import {Observable} from "rxjs";
import {AuthenticationService} from "../user/authentication.service";

/**
 * @ignore
 */
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {

  constructor(private _authService: AuthenticationService) {
  }

  get isLoggedIn(): Observable<boolean> {
    return this._authService.isLoggedIn$;
  }
}
