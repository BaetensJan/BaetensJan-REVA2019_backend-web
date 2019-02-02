import {Observable} from 'rxjs/Observable';
import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators} from '@angular/forms';
import {HttpErrorResponse} from '@angular/common/http';
import {map} from 'rxjs/operators';
import {InvitationService} from "../invitation.service";
import {SchoolDataService} from "../../../schools/school-data.service";
import {AuthenticationService} from "../../authentication.service";

@Component({
  selector: 'app-invite',
  templateUrl: './invite.component.html',
  styleUrls: ['./invite.component.css']
})
export class InviteComponent implements OnInit {

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
   * @param _invitationService
   * @param _schoolDataService
   * @param _authenticationService
   * @param router
   * @param fb
   */
  constructor(
    private _invitationService: InvitationService,
    private _schoolDataService: SchoolDataService,
    private _authenticationService: AuthenticationService,
    private router: Router,
    private fb: FormBuilder
  ) {
  }

  /**
   * Setup for username registration form
   */
  ngOnInit() {
    this.user = this.fb.group({
      name: [
        '',
        [Validators.required,
          Validators.minLength(1)],
        this.serverSideValidateUsername()
      ],
      surname: [
        '',
        [Validators.required,
          Validators.minLength(1)],
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
    this._invitationService
      .createTeacher(this.user.value.email, this.user.value.name, this.user.value.surname, this.user.value.schoolName)
      .subscribe(
        val => {
          if (val) {
            this.router.navigate(['/']);
          }
          console.log(val);
        },
        (error: HttpErrorResponse) => {
          this.errorMsg = `Er is iets fout gegaan bij het aanmaken van de leerkracht & school. Gelieve het later opnieuw te proberen.`;
        }
      );
  }
}
