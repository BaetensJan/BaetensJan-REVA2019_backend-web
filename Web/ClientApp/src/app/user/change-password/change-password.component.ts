import {Component, OnInit} from '@angular/core';
import {AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators} from "@angular/forms";
import {AuthenticationService} from "../authentication.service";
import {Router} from "@angular/router";

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

  constructor(
    private _authenticationService: AuthenticationService,
    private router: Router,
    private fb: FormBuilder
  ) {
  }

  ngOnInit() {
    this.passwordControl = this.fb.group(
      {
        currentPassword: ['', [Validators.required, passwordValidator(8)]], // keep length at 6 (backend
        // creates a password of length equal to 6).
        password: ['', [Validators.required, passwordValidator(8)]],
        confirmPassword: ['', Validators.required]
      },
      {validator: comparePasswords}
    )
  }

  onSubmit() {
    this._authenticationService.changePassword(this.passwordControl.value.currentPassword, this.passwordControl.value.password).subscribe();
    this.router.navigate(['/wachtwoord-vergeten-confirmation']);
  }

}
