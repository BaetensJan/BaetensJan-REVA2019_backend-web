import {Observable} from 'rxjs/Observable';
import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators} from '@angular/forms';
import {HttpErrorResponse} from '@angular/common/http';
import {map} from 'rxjs/operators';
import {InvitationService} from "../invitation.service";
import {SchoolDataService} from "../../schools/school-data.service";
import {AuthenticationService} from "../../user/authentication.service";
import {of as observableOf} from 'rxjs';

@Component({
  selector: 'app-invite-request',
  templateUrl: './invite-request.component.html',
  styleUrls: ['./invite-request.component.css']
})

/**
 * This component is used by a User to send a TeacherRequest to join the platform,
 * or by an admin in order to edit an existing TeacherRequest.
 */
export class InviteRequestComponent implements OnInit {

  /**
   * @ignore
   */
  public user: FormGroup;
  /**
   * @ignore
   */
  public errorMsg: string;

  public update: boolean = false;

  private requestId: number;

  // we need to cache this email when update a request in order for the emailvalidator,
  // otherwise it will tell us that this email was already taken (in database).
  private _email: string;
  // we need to cache this schoolName when update a request in order for the schoolNameValidator,
  // otherwise it will tell us that this email was already taken (in database).
  private _schoolName: string;

  /**
   * Constructor
   *
   * @param _invitationService
   * @param _schoolDataService
   * @param _authenticationService
   * @param router
   * @param route
   * @param fb
   */
  constructor(
    private _invitationService: InvitationService,
    private _schoolDataService: SchoolDataService,
    private _authenticationService: AuthenticationService,
    private router: Router,
    private fb: FormBuilder,
    private route?: ActivatedRoute
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
          Validators.minLength(1), Validators.maxLength(30)]
        // this.serverSideValidateUsername()
      ],
      surname: [
        '',
        [Validators.required,
          Validators.minLength(1), Validators.maxLength(30)]
      ],
      schoolName: [
        '',
        [Validators.required,
          Validators.minLength(1), Validators.maxLength(30),
          this.schoolNamePatternValidator()],
        this.serverSideValidateSchoolName()
      ],
      email: [
        '',
        Validators.compose([
          Validators.required, Validators.minLength(1), Validators.maxLength(60),
          this.emailPatternValidator()]),
        this.serverSideValidateEmail()
      ],
      note: ''
    });

    if (this._authenticationService.isModerator$.getValue()) {
      this.route.queryParams.subscribe(params => {
        // Defaults to 0 if no query param provided.
        this.requestId = params['requestId'] || -1;
        if (this.requestId != -1) {
          this._invitationService.getTeacherRequest(this.requestId).subscribe(value => {
            if (!value) this.router.navigate(["/"]);
            else {
              this.update = true;

              this._email = value.email;
              this._schoolName = value.schoolName;
              this.user.patchValue({
                name: value.name,
                surname: value.surname,
                schoolName: value.schoolName,
                note: value.note,
                email: value.email
              });
            }
          });
        }
      });
    }
  }

  emailPatternValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } => {
      const email = control.value;
      let regexp = new RegExp(
        /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/);
      //"^[^\s@]+@[^\s@]+\.[^\s@]{2,}$");
      let correctInput = regexp.test(email);
      return correctInput ? null : {wrongInput: true};
    };
  }

  schoolNamePatternValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } => {
      const schoolName: string = control.value;
      return schoolName.indexOf(".") < 0 ? null : {wrongInput: true};
    };
  }

  // /**
  //  * Checks Username for availability
  //  */
  // serverSideValidateUsername(): (control: AbstractControl) => Observable<{ [p: string]: any }> {
  //   return (control: AbstractControl): Observable<{ [key: string]: any }> => {
  //     return this._authenticationService
  //       .checkUserNameAvailability(control.value)
  //       .pipe(
  //         map(available => {
  //           if (available) {
  //             return null;
  //           }
  //           return {userAlreadyExists: true};
  //         })
  //       );
  //   };
  // }

  /**
   * Checks if schoolName already exists in database
   */
  serverSideValidateSchoolName(): (control: AbstractControl) => Observable<{ [p: string]: any }> {
    return (control: AbstractControl): Observable<{ [key: string]: any }> => {

      if (this.update && control.value == this._schoolName) {
        return observableOf(null);
      }

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

  serverSideValidateEmail(): (control: AbstractControl) => Observable<{ [p: string]: any }> {
    return (control: AbstractControl): Observable<{ [key: string]: any }> => {

      if (this.update && control.value == this._email) return observableOf(null);

      return this._authenticationService
        .checkEmailAvailability(control.value)
        .pipe(
          map(available => {
            if (available) {
              return null;
            }
            return {emailAlreadyExists: true};
          })
        );
    };
  }

  /**
   * Click event to submit user information to the backend for registration
   */
  onSubmit() {
    // admin updates a request.
    if (this.update) {
      this._invitationService
        .updateRequest(this.requestId, this.user.value.email, this.user.value.name, this.user.value.surname, this.user.value.schoolName, this.user.value.note)
        .subscribe(
          val => {
            if (val) {
              this.router.navigate(['requests']);
            }
          },
          (error: HttpErrorResponse) => {
            this.errorMsg = `Er is iets fout gegaan bij de verwerking van uw aanvraag. Gelieve het later opnieuw te proberen.`;
          }
        );
    } else {
      this._invitationService
        .inviteRequest(this.user.value.email, this.user.value.name, this.user.value.surname, this.user.value.schoolName, this.user.value.note)
        .subscribe(
          val => {
            if (val) {
              this.router.navigate([this._authenticationService.isModerator$.getValue() ? 'requests' : '/']);
            }
          },
          (error: HttpErrorResponse) => {
            this.errorMsg = `Er is iets fout gegaan bij de verwerking van uw aanvraag. Gelieve het later opnieuw te proberen.`;
          }
        );
    }
  }
}
