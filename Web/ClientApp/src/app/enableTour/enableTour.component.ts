import {Component, OnInit} from '@angular/core';
import {AuthenticationService} from "../user/authentication.service";
import {Observable} from "rxjs";


@Component({
  selector: 'app-enableTour',
  templateUrl: './enableTour.component.html',
  styleUrls: ['./enableTour.component.css']
})
export class EnableTourComponent implements OnInit {

  public get enableTourButtonText() {
    return this._isTourEnabled == true ? "Tour Uitschakelen" : "Tour Inschakelen";
  }

  get isSuperAdmin(): Observable<boolean> {
    return this._authService.isSuperAdmin$;
  }

  private _isTourEnabled: boolean;

  get tourIsActive(){
    return this._isTourEnabled
  }

  constructor(private _authService: AuthenticationService,) {
  }

  ngOnInit(): void {
    this._authService.getTourStatus().subscribe(isEnabled => {
      this._isTourEnabled = isEnabled;
    });
  };

  buttonClicked() {
    this._isTourEnabled = !this._isTourEnabled;

    if (this._isTourEnabled == false) {
      this._authService.disableTour().subscribe();
    } else {
      this._authService.enableTour().subscribe();
    }
  }
}
