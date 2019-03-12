import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {GroupSharedService} from "../group-shared.service";
import {Group} from "../../models/group.model";
import {FormBuilder, FormControl, FormGroup} from "@angular/forms";
import {GroupsDataService} from "../groups-data.service";
import {Router} from "@angular/router";
import {BsModalRef, BsModalService} from "ngx-bootstrap";
import {School} from "../../models/school.model";
import {SchoolDataService} from "../../schools/school-data.service";

@Component({
  selector: 'app-update-group',
  templateUrl: './update-group.component.html',
  styleUrls: ['./update-group.component.css']
})
export class UpdateGroupComponent implements OnInit {

  private _group: Group;
  public changePassword: boolean = false;

  public get createGroup() {
    return this._groupSharedService.createGroup;
  }

  modalRef: BsModalRef; // modal that appears asking for confirmation to remove a member from a group.
  modalMessage: string;
  private _school: School;

  constructor(private _groupSharedService: GroupSharedService,
              private _groupDataService: GroupsDataService,
              private _schoolDataService: SchoolDataService,
              private modalService: BsModalService,
              private fb: FormBuilder,
              private router: Router) {
  }

  ngOnInit() {
    if (!this.createGroup) {
      if (!this._groupSharedService.group) {
        this.goToGroupsOverview();
      } else {
        this._group = this._groupSharedService.group;
      }
    } else {
      this._groupSharedService.component = this;
      this._groupSharedService.groupsDataService = this._groupDataService;
    }
  }

  get groupMembers(): string[] {
    return this._groupSharedService.groupMembers;
  }

  get groupForm(): FormGroup {
    return this._groupSharedService.groupForm;
  }

  get group() {
    return this._group;
  }

  get school(): School {
    return this._school;
  }

  public changePasswordClicked() {
    this.changePassword = !this.changePassword;
  }

  addNewMember() {
    this._groupSharedService.addNewMember();
  }

  /**
   * Remove member from 'to be created' Group.
   */
  removeMember(memberName) {
    this._groupSharedService.removeMember(memberName);
  }

  get passwordControl(): FormControl {
    return this._groupSharedService.passwordControl;
  }


  removeGroup(group) {
    this._groupDataService.removeGroup(group).subscribe(_ => {
      this.goToGroupsOverview();
    });
  }

  decline(): void {
    this._groupSharedService.decline();
    this.modalRef.hide();
  }

  confirm(): void {
    this._groupSharedService.confirm();
  }

  /**
   * METHODS THAT ARE ONLY USED WHEN UPDATING A GROUP
   */

  @Output() submit = new EventEmitter<boolean>();

  goToGroupsOverview() {
    this.router.navigate(["group/groups"]);
  }

  /**
   * When submitting the form in order to create a new group.
   */
  onSubmit() {
    const groupId = {"groupId": this._groupSharedService.group.id};
    const changePassword = {"changePassword": this.changePassword};
    const group =
      {
        groupId,
        changePassword,
        ...this._groupSharedService.getGroup()
      };

    console.log(group);

    this._groupDataService.updateGroup(group).subscribe(_ => {
      this.goToGroupsOverview();
    });
  }

}
