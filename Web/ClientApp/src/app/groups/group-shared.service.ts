import {Injectable} from '@angular/core';
import {Group} from "../models/group.model";
import {
  AbstractControl,
  FormBuilder,
  FormGroup, NgForm,
  ValidatorFn,
  Validators
} from "@angular/forms";
import {Observable, of} from "rxjs";
import {map} from "rxjs/operators";
import {GroupsDataService} from "./groups-data.service";

function passwordValidator(length: number): ValidatorFn {
  return (control: AbstractControl): { [key: string]: any } => {
    return control.value.length < length ?
      {
        passwordTooShort:
          {
            requiredLength: length,
            actualLength: control.value.length
          }
      } : null;
  };
}

function comparePasswords(control: AbstractControl): { [key: string]: any } {
  const password = control.get('groupPassword');
  const confirmPassword = control.get('confirmPassword');
  return password.value === confirmPassword.value
    ? null
    : {passwordsDiffer: true};
}

// function groupMembersCheck(minLength: number, maxLength: number): ValidatorFn {
//   return (control: AbstractControl): { [key: string]: any } => {
//     const count = control.get("groupMembers").value.length;
//     console.log(count);
//     return count > minLength && count < maxLength + 1 ? null : {groupsMembersError: true};
//   };
// }

@Injectable({
  providedIn: 'root'
})
export class GroupSharedService {
  private _schoolId: number;

  public memberToRemove: string = '';
  public groupToRemove: Group;

  public groupForm: FormGroup;
  private _groupMembers: string[] = []; // members that were added to the newly created group.

  private _formDirective: NgForm;
  public set formDirective(formDirective: NgForm) {
    this._formDirective = formDirective;
  };

  constructor(private _groupsDataService: GroupsDataService,) {
  };

  //todo: move to src/common folder
  /**
   * Check on whitespaces.
   *
   */
  static noWhitespaceValidator(control: AbstractControl): { [key: string]: any } {
    const containsWhitespaces = /\s/.test(control.value);
    return containsWhitespaces ? {whitespace: true} : null;
  };

  /**
   * Check if contains dots ('.').
   */
  static schoolNamePatternValidator(control: AbstractControl): { [key: string]: any } {
    const schoolName: string = control.value;
    return schoolName.indexOf(".") < 0 ? null : {wrongInput: true};
  };

  get groupMembers(): string[] {
    return this._groupMembers;
  };

  private _group: Group;
  get group(): Group {
    return this._group;
  };

  /**
   * If _createGroup is true then we want to create a Group,
   * otherwise we want to update a Group.
   */
  private _createGroup: boolean = false;

  public setCreateGroup(schoolId: number) {
    // reset Group and GroupMembers, otherwise it will still be initiated if browser does not refresh.
    this.reset();

    this._schoolId = schoolId;

    this.prepareFormGroup();

    this.setPasswordGroupValidators();

    this._createGroup = true;
  };

  get createGroup(): boolean {
    return this._createGroup;
  };

  public updateGroup(schoolId, group: Group) {
    this._schoolId = schoolId;
    this._createGroup = false;
    this._group = group;
    this._groupMembers = group.members;

    this.prepareFormGroup();
    this.groupForm.get("passwordGroup").clearValidators();

    this.groupForm.patchValue({
      groupName: group.name
    });
  };

  private reset() {
    this._group = null;
    this._groupMembers = [];
    this._schoolId = null;
  };

  get schoolId(): number {
    return this._schoolId;
  };

  set schoolId(schoolId: number) {
    this._schoolId = schoolId;
  };

  private prepareFormGroup() {
    const fb = new FormBuilder();
    this.groupForm = fb.group({
        groupName: ['',
          [
            Validators.required,
            Validators.minLength(2),
            Validators.maxLength(20),
            GroupSharedService.schoolNamePatternValidator,
            GroupSharedService.noWhitespaceValidator,
          ],
          this.GroupNameAlreadyExists()
        ],
        passwordGroup: fb.group(
          {
            groupPassword: [''],
            confirmPassword: [''],
          }),
        groupMember: ['', [Validators.minLength(2), Validators.maxLength(20),
          GroupSharedService.schoolNamePatternValidator]],
        //groupMembers: [this.groupMembers]
      },
      //{validator: groupMembersCheck(0, 4)}
    );
  };

  get passwordGroup(): any {
    return this.groupForm.get('passwordGroup');
  };

  /**
   * FormBuilder 'passwordGroup' will be disabled when updating a group.
   * Only when clicked on 'change password' buttong will this method be called
   * to activate the validators of the 'passwordGroup'.
   */
  public setPasswordGroupValidators() {
    this.passwordGroup.setValidators(comparePasswords);
    this.passwordGroup.get('groupPassword').setValidators(
      [Validators.required, passwordValidator(6), GroupSharedService.noWhitespaceValidator]);
    this.passwordGroup.get('confirmPassword').setValidators(Validators.required);
  };

  /**
   * After submit of a group (group created), the fields and groupForm controls have
   * to be reset.
   */
  public resetGroupForm() {
    // reset all fields and attributes of group creation.
    this._groupMembers = [];

    // reset form
    this.groupForm.reset({
      groupName: '',
      groupMember: '',
      passwordGroup: {
        groupPassword: '',
        confirmPassword: '',
      },
    });
  };


  /**
   * add a member to a new group.
   */
  addNewMember() {
    let memberName = String(this.groupForm.value.groupMember.toString());
    this._groupMembers.push(memberName);
    this.groupForm.controls['groupMember'].setValue("");
  };

  /**
   * Checks GroupName for availability
   */
  GroupNameAlreadyExists(): (control: AbstractControl) => Observable<{ [p: string]: any }> {
    return (control: AbstractControl): Observable<{ [key: string]: any }> => {

      // no need to check if username exists when updating a group AND input value == groupName.
      let controlValue = control.value;
      if (!this._createGroup && this._group && this._group.name == controlValue) {
        return of(null);
      }

      return this._groupsDataService
        .checkGroupNameAvailability(/*this._schoolId.toString(), */control.value)
        .pipe(map(available => {
            if (available) {
              return null;
            }
            return {groupAlreadyExists: true};
          })
        );
    };
  };

  /**
   * When Teacher confirms to remove a group / member of group in the Modal (popup).
   *
   * Parameter (optional): the component Object that calls this method.
   * Expected when removing a Group.
   */
  confirm(): void {
    if (this.memberToRemove != '') {
      this.removeMember(this.memberToRemove);
    }
  };

  public removeMember(memberToRemove): void {
    if (!this._createGroup) {
      this._groupsDataService.removeMember(this._group.id, this.memberToRemove).subscribe(_ => {
        this.removeMemberOutOfMembersList(memberToRemove)
      });
    } else {
      this.removeMemberOutOfMembersList(memberToRemove);
    }
  };

  private removeMemberOutOfMembersList(memberToRemove: string) {
    const index = this._groupMembers.indexOf(memberToRemove, 0);
    if (index > -1) {
      this._groupMembers.splice(index, 1);
    }
    this.memberToRemove = '';
  }

  decline(): void {
    this.memberToRemove = '';
    this.groupToRemove = null;
  };

  getGroup(): any {
    return {
      'schoolId': this._schoolId,
      'name': this.groupForm.value.groupName,
      'password': this.passwordGroup.get('groupPassword').value,
      'members': this._groupMembers
    };
  };
}
