import {Component, OnInit} from '@angular/core';
import {AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators} from "@angular/forms";
import {AuthenticationService} from "../authentication.service";
import {Router} from "@angular/router";
import {HttpErrorResponse} from "@angular/common/http";

/**
 * Validator for password length
 *
 * @param length
 */
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
 * Check on whitespaces.
 *
 */
function noWhitespaceValidator(control: AbstractControl): { [key: string]: any } {
  const containsWhitespaces = /\s/.test(control.value);
  return containsWhitespaces ? {whitespace: true} : null;
}

/**
 * Check if new password is different to old password.
 *
 * @param control
 */
function differentPassword(control: AbstractControl): { [key: string]: any } {
  const differentPassword = control.get('password').value !== control.get('currentPassword').value;
  return differentPassword ? null : {differentPassword: true};
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
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent implements OnInit {
  /**
   * @ignore
   */
  public passwordControl: FormGroup;
  /**
   * @ignore
   */
  public errorMsg: string;
  private _wrongPassword: boolean = false;
  public get wrongPassword(){
    return this._wrongPassword;
  }

  constructor(
    private _authenticationService: AuthenticationService,
    private router: Router,
    private fb: FormBuilder
  ) {
  }

  ngOnInit() {
    this.passwordControl = this.fb.group(
      {
        currentPassword: ['', [Validators.required, passwordValidator(8)]],
        password: ['',
          [
            Validators.required,
            passwordValidator(8),
            noWhitespaceValidator,
          ]
        ],
        confirmPassword: ['', Validators.required],
      },
      {validator: [comparePasswords, differentPassword]}
    )
  }

  onSubmit() {
    this._wrongPassword = false;

    this._authenticationService.changePassword(this.passwordControl.value.currentPassword,
      this.passwordControl.value.password).subscribe(value => {
        this.router.navigate(['/wachtwoord-vergeten-confirmation']);
      },
      (err: HttpErrorResponse) => {
        // catched error in authenticationService ChangePassword method (catchError).
        if (err.message === "wrong password"){
          this.errorMsg = "Gelieve het correct, huidig wachtwoord in te vullen.";
          this._wrongPassword = true;
        }
      }
    );
  }
}
