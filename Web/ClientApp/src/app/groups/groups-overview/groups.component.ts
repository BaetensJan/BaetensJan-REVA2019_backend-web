import {Component, TemplateRef} from '@angular/core';
import {GroupsDataService} from "../groups-data.service";
import {Group} from "../../models/group.model";
import {School} from "../../models/school.model";
import {Observable} from "rxjs/Rx";
import {PageChangedEvent} from 'ngx-bootstrap/pagination';
import {AbstractControl, FormBuilder, FormGroup, Validators} from "@angular/forms";
import {BsModalRef, BsModalService} from "ngx-bootstrap";
import {AppShareService} from "../../app-share.service";
import {SchoolDataService} from "../../schools/school-data.service";
import {GroupSharedService} from "../group-shared.service";
import {Router} from "@angular/router";
import {map} from "rxjs/operators";
import {AuthenticationService} from "../../user/authentication.service";
import {AssignmentDataService} from "../../assignment/assignment-data.service";

@Component({
  selector: 'groups',
  templateUrl: './groups.component.html',
  styleUrls: ['./groups.component.css'],
  animations: []
})
export class GroupsComponent {
  memberToRemove = {name: "", group: null}; // member that should be removed from a group.
  modalRef: BsModalRef; // modal that appears asking for confirmation to remove a member from a group.
  modalMessage: string;

  private _isAdmin: boolean = false;
  get isAdmin(): boolean {
    return this._isAdmin;
  }

  currentPage; // the current page in the pagination (e.g. page 1, 2 ...)
  private _groups: Group[]; // array containing the groups that fits the filter.
  private _returnedArray: Group[]; // array containing the groups (maxNumberOfGroupsPerPage) that are shown on the current page.
  /**
   * Getter for groups
   */
  get returnedArray(): Group[] {
    return this._returnedArray;
  }

  private _filteredGroups: Group[]; // array containing all groups that meet the filter
  /**
   * Getter for groups
   */
  get filteredGroups(): Group[] {
    return this._filteredGroups;
  }

  maxNumberOfGroupsPerPage = 5; // amount of groups that will be showed on the current page, to keep the page neat.
  newMemberName: string = ""; // value of input-field in an already existing group. Will be used to add a new member to an existing group.
  createGroupClicked: boolean = false; // if the + button (for creating a group) was clicked.
  groupMembers: string[]; // members that were added to the newly created group.
  groupForm: FormGroup;
  filterValue: string = "";

  /**
   * Constructor
   * @param router
   * @param _groupsDataService
   * @param _groupsSharedService
   * @param _assignmentDataService
   * @param _schoolDataService
   * @param fb
   * @param _modalService
   * @param _appShareService
   * @param _authService
   */
  constructor(private router: Router,
              private _groupsDataService: GroupsDataService,
              private _groupsSharedService: GroupSharedService,
              private _schoolDataService: SchoolDataService,
              private _assignmentDataService: AssignmentDataService,
              private fb: FormBuilder,
              private _modalService: BsModalService,
              private _appShareService: AppShareService,
              private _authService: AuthenticationService) {
  }

  private _applicationStartDate: Date;

  get applicationStartDate(): Date {
    return this._applicationStartDate;
  }

  /**
   * when the current date is after of equal to the appStartDate -> returns true
   */
  get appStartDateExpired(): boolean {
    return this._applicationStartDate <= new Date();
  }

  private _filterOnGroupName: boolean = true; // current text in filterOption button

  get filterOnGroupName(): boolean {
    return this._filterOnGroupName;
  }

  set filterOnGroupName(value) {
    this._filterOnGroupName = value;
  }

  private _school: School;

  /**
   * Getter for school
   */
  get school(): School {
    return this._school;
  }

  ngOnInit(): void {
    let currentUser = AuthenticationService.parseJwt(localStorage.getItem("currentUser"));
    let schoolId = currentUser.school;
    this._isAdmin = currentUser.isAdmin;
    if (this._isAdmin) {
      this._schoolDataService.schools().subscribe((value: School[]) => {
        this._groups = [];
        value.forEach(school => {
          school.groups.forEach((group: Group) => {
            group.name = school.name + " " + group.name;
            this._groups.push(group);
          });
        });

        this.initiateArrays();
      });
    } else {
      this._schoolDataService.getSchool(schoolId).subscribe(value => {
        /*Todo If teacher has multiple and different schools*/
        this._school = value;
        this.prepareFormGroup();

        this._groups = this._school.groups;

        this.initiateArrays();
      });
    }
    this.groupMembers = [];
    this._assignmentDataService.getApplicationStartDate().subscribe(value => this._applicationStartDate = new Date(value));
  }

  updateGroup(group: Group) {
    this._groupsSharedService.updateGroup(this._school.id, group);
    this.router.navigate(["/group/updateGroup"]);
  }


  removeMemberFromAlreadyCreatedGroup(group, memberName) {
    this._groupsDataService.removeMember(group.id, memberName).subscribe(value => {
      const index = group.members.indexOf(memberName, 0);
      if (index > -1) {
        group.members.splice(index, 1);
      }
    });
  }

  removeGroup(group) {
    this._groupsDataService.removeGroup(group).subscribe(value => {
      let index = this._groups.indexOf(group, 0);
      if (index > -1) {
        this._groups.splice(index, 1);
      }
      index = this._returnedArray.indexOf(group, 0);
      if (index > -1) {
        this._returnedArray.splice(index, 1);
      }
    });
  }

  decline(): void {
    this.memberToRemove.name = "";
    this.memberToRemove.group = null;
    this.modalRef.hide();
  }

  /**
   * Shows a message when a group was successfully created.
   * @param groupName
   */
  public add(groupName: string): void {
    this._appShareService.addAlert(
      {
        type: 'success',
        msg: `De nieuwe groep met groepsnaam ${groupName} werd succesvol toegevoegd.}`,
        timeout: 5000
      });
  }

  /** FILTER **/
  public filter(token: string) {
    token = token.toLowerCase();
    if (!token) {
      this._filteredGroups = this._groups;
    } else {
      if (this._filterOnGroupName) {
        this._filteredGroups = this._groups.filter((group: Group) => {
          return group.name.toLowerCase().includes(token);
        });
      } else {
        this._filteredGroups = [];
        this._groups.forEach((group: Group) => {
          for (let i in group.members) {
            let member = group.members[i];
            if (member.toLowerCase().includes(token)) {
              this._filteredGroups.push(group);
              break;
            }
          }
          // return group.members.toString().includes(token);
        });
      }
    }
    this._returnedArray = this._filteredGroups.slice(0, this.maxNumberOfGroupsPerPage);
    this.currentPage = 1; // switches current page in pagination back to page 1
  }

  /** MODAL / POPUP **/
  openModal(template: TemplateRef<any>, groupId: number, memberName?: string) {
    if (memberName) {
      if (groupId != -1) {
        let group = this.findGroupWithId(groupId);
        this.memberToRemove.group = group;
        this.modalMessage = `Ben je zeker dat ${memberName} verwijderd mag worden uit ${group.name}?`;
      } else { // delete member of not-yet-created group.
        this.modalMessage = `Ben je zeker dat ${memberName} verwijderd mag worden uit de groep?`;
      }
      this.memberToRemove.name = memberName;
    } else { // deleting of a group.
      let group = this.findGroupWithId(groupId);
      this.memberToRemove.group = group;
      this.modalMessage = `Ben je zeker dat de groep met groepsnaam ${group.name} verwijderd mag worden? 
      De ingediende opdrachten van deze groep worden hierdoor ook verwijderd.`;
    }
    this.modalRef = this._modalService.show(template, {class: 'modal-sm'});
  }

  /**
   * When Teacher confirms to remove a group / member of group in the Modal (popup).
   */
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

  /**
   * Sorts the groups alphabetically
   */
  initiateArrays() {
    this._groups.sort(/*(a, b) => a.name > b.name ? 1 : -1*/);
    this._filteredGroups = this._groups;
    this._returnedArray = this._filteredGroups.slice(0, this.maxNumberOfGroupsPerPage);
  }

  prepareFormGroup() {
    this.groupForm = this.fb.group({
      groupName: ['', [Validators.required, Validators.minLength(2)], this.GroupNameAlreadyExists()],
      groupPassword: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(35)]],
      groupMember: ['', /*[Validators.minLength(1)]*/]
    });
  }

  /**
   * Checks GroupName for availability
   */
  GroupNameAlreadyExists(): (control: AbstractControl) => Observable<{ [p: string]: any }> {
    return (control: AbstractControl): Observable<{ [key: string]: any }> => {
      return this._groupsDataService
        .checkGroupNameAvailability(this._school.id.toString(), control.value)
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
   * When a user clicks on the next page pagination button,
   * it will show the next groups.
   * @param event
   */
  pageChanged(event: PageChangedEvent): void {
    this.filterValue = "";
    const startItem = (event.page - 1) * event.itemsPerPage;
    const endItem = event.page * event.itemsPerPage;
    this._returnedArray = this._filteredGroups.slice(startItem, endItem);
  }

  /**
   * add a member to an already existing group.
   * @param groupId
   */
  addMember(groupId: number) {
    this._groupsDataService.addMember(groupId, this.newMemberName)
      .subscribe(value => {
        this.findGroupWithId(groupId).members.push(this.newMemberName);
        this.newMemberName = "";
      });
  }

  /**
   * add a member to a new group.
   */
  addNewMember() {
    let memberName = String(this.groupForm.value.groupMember.toString());
    this.groupMembers.push(memberName);
    this.groupForm.controls['groupMember'].setValue(""); //.reset();
  }

  /**
   * check if the input field for adding a new member to a group is not empty,
   * Used by the add-button, so if it is empty -> hide the add-button.
   */
  isNewMemberInputEmpty(): boolean {
    return this.newMemberName === "";
  }

  /**
   * When submitting the form in order to create a new group.
   */
  onSubmit() {
    // let newGroup = new Group(this.groupForm.value.groupName, null, this.groupMembers);
    let newGroup = {
      "name": this.groupForm.value.groupName,
      "password": this.groupForm.value.groupPassword,
      "members": this.groupMembers
    };
    this._groupsDataService.addNewGroup(this.school.id, newGroup).subscribe(value => {
      this._groups.push(value);
      this.initiateArrays();
      this.groupMembers = [];
      this.groupForm.controls['groupName'].setValue("");
      this.groupForm.controls['groupPassword'].setValue("");
      // show alert (modal)
      this.add(newGroup.name);
    });
  }

  private findGroupWithId(groupId: number) {
    let group = this._groups.find(g => g.id == groupId);
    return group;
  }
}
