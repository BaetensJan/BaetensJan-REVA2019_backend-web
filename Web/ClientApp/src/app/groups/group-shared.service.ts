import {Injectable, TemplateRef} from '@angular/core';
import {Group} from "../models/group.model";
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup, NgForm,
  ValidatorFn,
  Validators
} from "@angular/forms";
import {Observable, of} from "rxjs";
import {map} from "rxjs/operators";
import {GroupsDataService} from "./groups-data.service";
import {BsModalService} from "ngx-bootstrap";

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

/**
 * Check on whitespaces.
 *
 */
function noWhitespaceValidator(control: AbstractControl): { [key: string]: any } {
  const containsWhitespaces = /\s/.test(control.value);
  return containsWhitespaces ? {whitespace: true} : null;
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

  private _group: Group;
  private _schoolId: number;

  public memberToRemove: string = '';
  public groupToRemove: Group;

  public groupForm: FormGroup;
  private _groupMembers: string[] = []; // members that were added to the newly created group.

  private _formDirective: NgForm;
  public set formDirective(formDirective: NgForm) {
    this._formDirective = formDirective;
  }

  constructor(private _groupsDataService: GroupsDataService,) {
  }

  get groupMembers(): string[] {
    return this._groupMembers;
  }

  get group(): Group {
    return this._group;
  }

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
  }

  get createGroup(): boolean {
    return this._createGroup;
  }

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
  }

  private reset() {
    this._group = null;
    this._groupMembers = [];
    this._schoolId = null;
  }

  get schoolId(): number {
    return this._schoolId;
  }

  set schoolId(schoolId: number) {
    this._schoolId = schoolId;
  }

  private prepareFormGroup() {
    const fb = new FormBuilder();
    this.groupForm = fb.group({
        groupName: ['',
          [Validators.required, Validators.minLength(2)],
          this.GroupNameAlreadyExists()],
        passwordGroup: fb.group(
          {
            groupPassword: [''],
            confirmPassword: [''],
          }),
        groupMember: ['', Validators.minLength(2)],
        //groupMembers: [this.groupMembers]
      },
      //{validator: groupMembersCheck(0, 4)}
    );
  }

  public setPasswordGroupValidators() {
    this.groupForm.get("passwordGroup").setValidators(comparePasswords);
    this.passwordControl.setValidators([Validators.required, passwordValidator(6), noWhitespaceValidator]);
    this.groupForm.get("passwordGroup").get("confirmPassword").setValidators(Validators.required);
  }

  public resetGroupForm() {
    // reset all fields and attributes of group creation.
    while (this._groupMembers.length) this._groupMembers.pop();

    // also reset the errors
    this._formDirective.resetForm();

    // reset form
    this.groupForm.reset();
  }

  get passwordControl(): FormControl {
    return <FormControl>this.groupForm.get('passwordGroup').get('groupPassword');
  }

  /**
   * add a member to a new group.
   */
  addNewMember() {
    let memberName = String(this.groupForm.value.groupMember.toString());
    this._groupMembers.push(memberName);
    this.groupForm.controls['groupMember'].setValue("");
  }

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
        .checkGroupNameAvailability(this._schoolId.toString(), control.value)
        .pipe(map(available => {
            if (available) {
              return null;
            }
            return {groupAlreadyExists: true};
          })
        );
    };
  }

  /**
   * When Teacher confirms to remove a group / member of group in the Modal (popup).
   *
   * Parameter (optional): the component Object that calls this method.
   * Expected when removing a Group.
   */
  confirm(component?: any): void {
    if (component && this.memberToRemove != '') {
      this.removeMember(this.memberToRemove);
      this.memberToRemove = '';
    } else {
      this._groupsDataService.removeGroup(this.groupToRemove).subscribe(value => {
        component.removeGroup(this.groupToRemove);
      });
    }
  }

  public removeMember(memberToRemove): void {
    const index = this._groupMembers.indexOf(memberToRemove, 0);
    if (index > -1) {
      this._groupMembers.splice(index, 1);
    }
  }

  decline(): void {
    this.memberToRemove = '';
    this.groupToRemove = null;
  }

  getGroup(): any {
    return {
      'schoolId': this._schoolId,
      'name': this.groupForm.value.groupName,
      'password': this.passwordControl.value,
      'members': this._groupMembers
    };
  }

  /** MODAL / POPUP **/
  openModal(component: any, modalService: BsModalService, template: TemplateRef<any>, group: Group, memberName?: string): any {
    let modalMessage;

    if (memberName) {
      modalMessage = `Ben je zeker dat ${memberName} verwijderd mag worden uit de groep?`;

      this.memberToRemove = memberName;
    } else { // deleting of a group.
      this.groupToRemove = group;
      modalMessage = `Ben je zeker dat de groep met groepsnaam ${group.name} verwijderd mag worden? 
      De ingediende opdrachten van deze groep worden hierdoor ook verwijderd.`;
    }
    component.modalMessage = modalMessage;
    component.modalRef = modalService.show(template, {class: 'modal-sm'});
  }
}
