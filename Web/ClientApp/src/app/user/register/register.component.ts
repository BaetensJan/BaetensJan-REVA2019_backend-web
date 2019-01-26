import {Observable} from 'rxjs/Observable';
import {AuthenticationService} from '../authentication.service';
import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators} from '@angular/forms';
import {HttpErrorResponse} from '@angular/common/http';
import {map} from 'rxjs/operators';
import {SchoolDataService} from "../../schools/school-data.service";

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
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  /**
   * @ignore
   */
  public user: FormGroup;
  /**
   * @ignore
   */
  public errorMsg: string;

  /**
   * Getter for password group
   *
   */
  get passwordControl(): FormControl {
    return <FormControl>this.user.get('passwordGroup').get('password');
  }

  /**
   * Constructor
   *
   * @param _authenticationService
   * @param _schoolDataService
   * @param router
   * @param fb
   */
  constructor(
    private _authenticationService: AuthenticationService,
    private _schoolDataService: SchoolDataService,
    private router: Router,
    private fb: FormBuilder
  ) {
  }

  /**
   * Setup for username registration form
   */
  ngOnInit() {
    this.user = this.fb.group({
      username: [
        '',
        [Validators.required,
          Validators.minLength(5)],
        this.serverSideValidateUsername()
      ],
      schoolName: [
        '',
        [Validators.required,
          Validators.minLength(5),
          this.schoolNamePatternValidator()],
        this.serverSideValidateSchoolName()
      ],
      email: [
        '', Validators.compose([
          Validators.required,
          this.emailPatternValidator()])
      ],
      passwordGroup: this.fb.group(
        {
          password: ['', [Validators.required, passwordValidator(8)]],
          confirmPassword: ['', Validators.required]
        },
        {validator: comparePasswords}
      )
    });
  }

  emailPatternValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } => {
      const email = control.value;
      let regexp = new RegExp(
        /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/);
      //"^[^\s@]+@[^\s@]+\.[^\s@]{2,}$");
      let correctInput = regexp.test(email);
      console.log(correctInput);
      return correctInput ? null : { wrongInput: true };
    };
  }

  schoolNamePatternValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } => {
      const schoolName: string = control.value;
      return schoolName.indexOf(".") < 0 ? null : { wrongInput: true };
    };
  }

  /**
   * Checks Username for availability
   */
  serverSideValidateUsername(): (control: AbstractControl) => Observable<{ [p: string]: any }> {
    return (control: AbstractControl): Observable<{ [key: string]: any }> => {
      return this._authenticationService
        .checkUserNameAvailability(control.value)
        .pipe(
          map(available => {
            if (available) {
              return null;
            }
            return {userAlreadyExists: true};
          })
        );
    };
  }

  /**
   * Checks if schoolName already exists in database
   */
  serverSideValidateSchoolName(): (control: AbstractControl) => Observable<{ [p: string]: any }> {
    return (control: AbstractControl): Observable<{ [key: string]: any }> => {
      return this._schoolDataService
        .checkSchoolNameAvailability(control.value)
        .pipe(
          map(available => {
            if (available) {
              return null;
            }
            return {schoolAlreadyExists: true};
          })
        );
    };
  }

  /**
   * Click event to submit user information to the backend for registration
   */
  onSubmit() {
    this._authenticationService
      .register(this.user.value.email, this.user.value.username, this.passwordControl.value, this.user.value.schoolName)
      .subscribe(
        val => {
          if (val) {
            this.router.navigate(['/']);
          }
          console.log(val);
        },
        (error: HttpErrorResponse) => {
          this.errorMsg = `Error ${
            error.status
            } while trying to register user ${this.user.value.username}: ${
            error.error
            }`;
        }
      );
  }
}
