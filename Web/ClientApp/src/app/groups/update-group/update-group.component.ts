import {AfterViewInit, Component, EventEmitter, OnInit, Output, ViewChild} from '@angular/core';
import {GroupSharedService} from "../group-shared.service";
import {Group} from "../../models/group.model";
import {FormBuilder, FormControl, FormGroup, NgForm} from "@angular/forms";
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
export class UpdateGroupComponent implements OnInit, AfterViewInit {
  @ViewChild('formDirective') private _formDirective: NgForm;

  private _group: Group;
  public changePassword: boolean = false;

  // counts number of times the changePassword Button has been clicked.
  // private _changePasswordCounter: number = 0;

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
      this._groupSharedService.groupsDataService = this._groupDataService;
    }
  }

  ngAfterViewInit(): void {
    this._groupSharedService.formDirective = this._formDirective;
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

  public changePasswordClicked() {
    this.changePassword = !this.changePassword;
    // this._changePasswordCounter++;

    // when a Teacher clicks on the changePassword button for the first time => set password validators.
    // if (this._changePasswordCounter == 1) {
    this._groupSharedService.setPasswordGroupValidators();
    // }
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

  @Output() submit: EventEmitter<Group> = new EventEmitter();

  goToGroupsOverview() {
    this.router.navigate(["group/groups"]);
  }

  /**
   * When submitting the form in order to create a new group.
   */
  onSubmit() {
    // if creation of new Group.
    if (this.createGroup) {
      this.submit.emit(this.group);
    } else { // updating of existing group.
      const groupId = this._groupSharedService.group.id;
      const changePassword = this.changePassword;
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
}
