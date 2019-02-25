import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators} from "@angular/forms";
import {HttpErrorResponse} from "@angular/common/http";
import {AuthenticationService} from "../authentication.service";

function passwordValidator(length: number): ValidatorFn {
  return (control: AbstractControl): { [key: string]: any } => {
    return control.value.length < length
      ? {
        passwordTooShort: {
          requiredLength: length,
          actualLength: control.value.length
        }
      }
      : null;
  };
}

/**
 * Compare function for passwords
 *
 * @param control
 */
function comparePasswords(control: AbstractControl): { [key: string]: any } {
  const password = control.get('password');
  const confirmPassword = control.get('confirmPassword');
  return password.value === confirmPassword.value
    ? null
    : {passwordsDiffer: true};
}

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit {
  /**
   * @ignore
   */
  public passwords: FormGroup;
  /**
   * @ignore
   */
  public errorMsg: string;
  /**
   * @ignore
   */
  private _code: string;
  /**
   * @ignore
   */
  private _email: string;

  constructor(private _authenticationService: AuthenticationService,
              private router: Router,
              private route: ActivatedRoute,
              private fb: FormBuilder) {
  }

  ngOnInit() {
    this.route.queryParams
      .subscribe(params => {
        if (params['code'] && params['email']) {
          this._code = params.code;
          this._code = this._code.replace(/ /g, "+");
          this._email = params.email;
        } else {
          this.router.navigate(['/home']);
        }
      });
    this.passwords = this.fb.group(
      {
        password: ['', [Validators.required, passwordValidator(8)]],
        confirmPassword: ['', Validators.required]
      },
      {validator: comparePasswords}
    );
  }

  onSubmit() {
    this._authenticationService
      .resetPassword(this._email, this._code, this.passwords.value.password)
      .subscribe(
        val => {
          this.router.navigate(['/home']);
        },
        (error: HttpErrorResponse) => {
          this.errorMsg = `Error ${
            error.status
            } while trying to reset password: ${
            error.error
            }`;
        }
      );
  }

}
