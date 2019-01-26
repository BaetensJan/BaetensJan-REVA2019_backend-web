import {AuthenticationService} from '../authentication.service';
import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators} from '@angular/forms';
import {HttpErrorResponse} from '@angular/common/http';


/**
 * Validator for password length
 */
function passwordValidator(): ValidatorFn {
  return (control: AbstractControl): { [key: string]: any } => {
    return control.value.length < 12
      ? {passwordTooShort: {value: control.value.length}}
      : null;
  };
}

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  /**
   * @ignore
   */
  public user: FormGroup;
  /**
   * @ignore
   */
  public errorMsg: string;

  /**
   * Constructor
   *
   * @param authService
   * @param router
   * @param fb
   */
  constructor(
    private authService: AuthenticationService,
    private router: Router,
    private fb: FormBuilder
  ) {
  }

  /**
   * Setup for user form
   */
  ngOnInit() {
    this.user = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  /**
   * Click event authenticates user to backend and logs in user
   */
  onSubmit() {
    this.authService
      .login(this.user.value.username, this.user.value.password)
      .subscribe(
        val => {
          if (val) {
            if (this.authService.redirectUrl) {
              this.router.navigateByUrl(this.authService.redirectUrl);
              this.authService.redirectUrl = undefined;
            } else {
              this.router.navigate(['/']);
            }
          } else {
            this.errorMsg = `Could not login`;
          }
        },
        (err: HttpErrorResponse) => {
          if (err.error instanceof Error) {
            this.errorMsg = `Error while trying to login user ${
              this.user.value.username
              }: ${err.error.message}`;
          } else {
            this.errorMsg = `Error ${err.status} while trying to login user ${
              this.user.value.username
              }`;//: ${err.error}`;
          }
        }
      );
  }
}
