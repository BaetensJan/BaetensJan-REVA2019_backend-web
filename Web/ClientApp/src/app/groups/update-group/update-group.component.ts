import {Component, OnInit, TemplateRef} from '@angular/core';
import {GroupSharedService} from "../group-shared.service";
import {Group} from "../../models/group.model";
import {AbstractControl, FormBuilder, FormGroup, Validators} from "@angular/forms";
import {GroupsDataService} from "../groups-data.service";
import {Observable} from "rxjs";
import {map} from "rxjs/operators";
import {Router} from "@angular/router";
import { of } from 'rxjs';
import {BsModalRef, BsModalService} from "ngx-bootstrap";
import {School} from "../../models/school.model";
import {SchoolDataService} from "../../schools/school-data.service";

@Component({
  selector: 'app-update-group',
  templateUrl: './update-group.component.html',
  styleUrls: ['./update-group.component.css']
})
export class UpdateGroupComponent implements OnInit {

  /**
   * @ignore
   */
  private _groupForm: FormGroup;
  private _group: Group;
  private _schoolId;
  memberToRemove = {name: "", group: null};
  modalRef: BsModalRef; // modal that appears asking for confirmation to remove a member from a group.
  modalMessage: string;
  _groups: Group[];
  groupMembers: String[];
  returnedArray: Group[];
  private _school: School;

  constructor(private _groupSharedService: GroupSharedService,
              private _groupDataService: GroupsDataService,
              private _schoolDataService: SchoolDataService,
              private modalService: BsModalService,
              private fb: FormBuilder,
              private router: Router) {
  }

  ngOnInit() {
    if (this._groupSharedService.edit == false || !this._groupSharedService.group)
      this.router.navigate(["groepen"]);
    else {
      this._schoolId = this._groupSharedService.schoolId;
      this._schoolDataService.getSchool(this._schoolId).subscribe(value => {
        /*Todo If teacher has multiple and different schools*/
        this._school = value;
      });
      this._group = this._groupSharedService.group;
      this.groupMembers = this._group.members;
      this.prepareForm(this._group);
    }
  }

  private prepareForm(group: Group) {
    this._groupForm = this.fb.group({
      groupName: [group.name, [Validators.required, Validators.minLength(5), Validators.maxLength(35)],
        this.GroupNameAlreadyExists()
      ],
      groupMember: [''],
      groupPassword: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(35)]]
    });
  }

  /**
   * Checks GroupName for availability
   */
  GroupNameAlreadyExists(): (control: AbstractControl) => Observable<{ [p: string]: any }> {
    return (control: AbstractControl): Observable<{ [key: string]: any }> => {
      if (control.value == this._group.name) return of(null);

      return this._groupDataService
        .checkGroupNameAvailability(this._schoolId, control.value)
        .pipe(
          map(available => {
            if (available) {
              return null;
            }
            return {GroupAlreadyExists: true};
          })
        );
    };
  }

  addNewMember(member: string): void {
    this._group.members.push(member);
    this.groupForm.controls['groupMember'].setValue("");
  }

  /**
   * When submitting the form in order to create a new group.
   */
  onSubmit() {
    let group = {
      "groupId": this._groupSharedService.group.id,
      "name": this._groupForm.value.groupName,
      "password": this._groupForm.value.groupPassword,
      "members": this._group.members //Todo check if max 5 members?
    };

    this._groupDataService.updateGroup(group).subscribe(value => {
      this.router.navigate(["groepen/overzicht"]);
    });
  }

  openModal(template: TemplateRef<any>, groupId: number, memberName?: string) {
    if (memberName) {
      if (groupId != -1) {
        //let group = this.findGroupWithId(groupId);
        this.memberToRemove.group = this._group;
        this.modalMessage = `Ben je zeker dat ${memberName} verwijderd mag worden uit ${this._group.name}?`;
      } else { // delete member of not-yet-created group.
        this.modalMessage = `Ben je zeker dat ${memberName} verwijderd mag worden uit de groep?`;
      }
      this.memberToRemove.name = memberName;
    }
    else { // deleting of a group.
      //let group = this.findGroupWithId(groupId);
      this.memberToRemove.group = this._group;
      console.log(groupId);

      this.modalMessage = `Ben je zeker dat de groep met groepsnaam ${this._group.name} verwijderd mag worden? De ingediende opdrachten van deze groep worden hierdoor ook verwijderd.`;

    }
    this.modalRef = this.modalService.show(template, {class: 'modal-sm'});
  }

  /*private findGroupWithId(groupId: number) {
    let group = this._groups.find(g => g.id == groupId);
    return group;
  }*/

  confirm(): void {
    if (this.memberToRemove.name != "") {
      if (this.memberToRemove.group != null) {
        this.removeMemberFromAlreadyCreatedGroup(this.memberToRemove.group, this.memberToRemove.name);
      }
      const index = this.groupMembers.indexOf(this.memberToRemove.name, 0);
      if (index > -1) {
        this.groupMembers.splice(index, 1);
      }
    } else { // member to remove is from an already existing group.
      this.removeGroup(this.memberToRemove.group);
    }
    this.memberToRemove.name = "";
    this.memberToRemove.group = null;

    this.modalRef.hide();
  }


  decline(): void {
    this.memberToRemove.name = "";
    this.memberToRemove.group = null;
    this.modalRef.hide();
  }

  removeMemberFromAlreadyCreatedGroup(group, memberName) {
    this._groupDataService.removeMember(group.id, memberName).subscribe(value => {
      console.log(value);
      const index = group.members.indexOf(memberName, 0);
      if (index > -1) {
        group.members.splice(index, 1);
      }
    });
  }

  removeGroup(group) {
    this._groupDataService.removeGroup(group).subscribe(value => {
      let index = this._groups.indexOf(group, 0);
      if (index > -1) {
        this._groups.splice(index, 1);
      }
      index = this.returnedArray.indexOf(group, 0);
      if (index > -1) {
        this.returnedArray.splice(index, 1);
      }
    });
  }

  get groupForm() {
    return this._groupForm;
  }

  get group() {
    return this._group;
  }

  get school(): School {
    return this._school;
  }
}
