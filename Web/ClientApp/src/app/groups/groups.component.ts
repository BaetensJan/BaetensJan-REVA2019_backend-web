import {Component, TemplateRef} from '@angular/core';
import {GroupsDataService} from "./groups-data.service";
import {Group} from "../models/group.model";
import {Observable} from "rxjs/Rx";
import {PageChangedEvent} from 'ngx-bootstrap/pagination';
import {AbstractControl, FormBuilder, FormGroup, Validators} from "@angular/forms";
import {BsModalRef, BsModalService} from "ngx-bootstrap";
import {AppShareService} from "../AppShareService";
import {School} from "../models/school.model";
import {SchoolDataService} from "../schools/school-data.service";
import {GroupSharedService} from "./group-shared.service";
import {Router} from "@angular/router";
import {map} from "rxjs/operators";
import {AssignmentDataService} from "../assignments/assignment-data.service";
import {AuthenticationService} from "../user/authentication.service";

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

  contentArray: Group[]; // array containing the groups that fits the filter.
  returnedArray: Group[]; // array containing the groups (maxNumberOfGroupsPerPage) that are showed on the current page.
  filteredGroups: Group[];

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

  private _filterText: string = "Groepsnaam"; // current text in filterOption button

  get filterText(): string {
    return this._filterText;
  }

  private _school: School;

  /**
   * Getter for school
   */
  get school(): School {
    return this._school;
  }

  _groups: Group[]; // array containing ALL the groups.

  /**
   * Getter for groups
   */
  get groups(): Group[] {
    return this._groups;
  }

  ngOnInit(): void {
    let currentUser = AuthenticationService.parseJwt(localStorage.getItem("currentUser"));
    let schoolId = currentUser.school;
    let isAdmin = currentUser.isAdmin;
    if (isAdmin == "True") {
      this._groupsDataService.allGroups.subscribe(value => {
        this._groups = value;
        this.contentArray = this._groups;
        this.filteredGroups = this._groups;
        this.initiateReturnedArray();
      });
    } else {
      this._schoolDataService.getSchool(schoolId).subscribe(value => {
        /*Todo If teacher has multiple and different schools*/
        this._school = value;
        this.prepareFormGroup();
        this._groups = this._school.groups;
        this.contentArray = this._groups;
        this.initiateReturnedArray();
        this.filteredGroups = this._groups
      });
    }
    this.groupMembers = [];
    this._assignmentDataService.getApplicationStartDate().subscribe(value => this._applicationStartDate = new Date(value));
  }

  updateGroup(group: Group) {
    this._groupsSharedService.updateGroup(this._school.id, group);
    this.router.navigate(["groepen/updateGroup"]);
  }


  removeMemberFromAlreadyCreatedGroup(group, memberName) {
    this._groupsDataService.removeMember(group.id, memberName).subscribe(value => {
      console.log(value);
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
      index = this.returnedArray.indexOf(group, 0);
      if (index > -1) {
        this.returnedArray.splice(index, 1);
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

  changeFilter() {
    if (this.filterText == "Groepslid") {
      this._filterText = "Groepsnaam";
    } else {
      this._filterText = "Groepslid";
    }
  }

  public filter(token: string) {
    console.log(token);
    if (this.filterText == "Groepsnaam") {
      this.filteredGroups = this._groups.filter((group: Group) => {
        return group.name.toLowerCase().startsWith(token.toLowerCase());
      });
    } else {
      this.filteredGroups = this._groups.filter((group: Group) => {
        return group.members.find((member: string) => member.toLowerCase().startsWith(token.toLowerCase()));
      });
    }
    this.returnedArray = this.filteredGroups.slice(0, this.maxNumberOfGroupsPerPage);
  }

  /**
   * Edits the contentArray (array that represents all the groups that should be presented) to
   * the current filter value (text that user typed in the filter field).
   */
  executeFilterQuery(filter?: string) {
    this.contentArray = [];
    if (filter) {
      this._groups.forEach(group => {
        let sub = group.name.toLowerCase().substr(0, filter.length);
        if (sub == filter.toLocaleLowerCase() /*|| group.members.includes(filter)*/) { //TODO eens members geimplementeerd is in backend, uit commentaar halen.
          this.contentArray.push(group);
        }
      });
    } else {
      this.contentArray = this._groups;
    }
    this.initiateReturnedArray();
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
   * initiates the returnedArray with the groups that should be presented on the current page.
   */
  initiateReturnedArray() {
    this.returnedArray = this.contentArray.sort((a, b) => a.name > b.name ? 1 : -1).slice(0, this.maxNumberOfGroupsPerPage);
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
    const startItem = (event.page - 1) * event.itemsPerPage;
    const endItem = event.page * event.itemsPerPage;
    this.returnedArray = this.contentArray.slice(startItem, endItem);
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
    console.log(this.groupMembers)
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
    console.log(newGroup.password);
    this._groupsDataService.addNewGroup(this.school.id, newGroup).subscribe(value => {
      console.log(value);
      this._groups.push(value);
      this.executeFilterQuery();
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
