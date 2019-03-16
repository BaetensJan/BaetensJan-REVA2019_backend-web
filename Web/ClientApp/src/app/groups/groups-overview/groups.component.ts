import {Component, TemplateRef, ViewChild} from '@angular/core';
import {GroupsDataService} from "../groups-data.service";
import {Group} from "../../models/group.model";
import {School} from "../../models/school.model";
import {PageChangedEvent} from 'ngx-bootstrap/pagination';
import {FormBuilder, FormGroup} from "@angular/forms";
import {BsModalRef, BsModalService} from "ngx-bootstrap";
import {AppShareService} from "../../app-share.service";
import {SchoolDataService} from "../../schools/school-data.service";
import {GroupSharedService} from "../group-shared.service";
import {Router} from "@angular/router";
import {AuthenticationService} from "../../user/authentication.service";
import {AssignmentDataService} from "../../assignment/assignment-data.service";

@Component({
  selector: 'groups',
  templateUrl: './groups.component.html',
  styleUrls: ['./groups.component.css'],
  animations: []
})
export class GroupsComponent {

  modalRef: BsModalRef; // modal that appears, asking for confirmation to remove a member from a group.
  modalMessage: string;

  private _createGroup: boolean = false;
  get createGroup(): boolean {
    return this._createGroup;
  }

  get groupForm(): FormGroup{
    return this._groupSharedService.groupForm;
  }

  private _isAdmin: string;
  get isAdmin(): boolean {
    return this._isAdmin == 'True';
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

  filterValue: string = "";

  /**
   * Constructor
   * @param router
   * @param _groupsDataService
   * @param _groupSharedService
   * @param _assignmentDataService
   * @param _schoolDataService
   * @param fb
   * @param _modalService
   * @param _appShareService
   * @param _authService
   */
  constructor(private router: Router,
              private _groupsDataService: GroupsDataService,
              private _groupSharedService: GroupSharedService,
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
    this._groupSharedService.setCreateGroup(schoolId);

    this._isAdmin = currentUser.isAdmin;

    if (this.isAdmin) {
      this._schoolDataService.schools().subscribe((value: School[]) => {
        this._groups = [];
        this._schoolDataService.schools().subscribe((schools: School[]) => {
          for (let i = 0; i < schools.length; i++) {
            let school = schools[i];
            for (let j = 0; j < school.groups.length; j++) {
              let group = school.groups[j];
              group.schoolName = school.name;

              this._groups.push(group);
            }
          }
          this.initiateArrays();
        });
      });
    } else {
      this._schoolDataService.getSchool(schoolId).subscribe((school: School) => {
        this._school = school;

        this._groups = this._school.groups;

        this.initiateArrays();
      });
    }
    this._assignmentDataService.getApplicationStartDate().subscribe(value => this._applicationStartDate = new Date(value));
  }

  public createGroupClicked() {
    this._createGroup = !this._createGroup;
  }

  public updateGroup(group: Group) {
    this._groupSharedService.updateGroup(this._school.id, group);
    this.router.navigate(["/group/updateGroup"]);
  }

  private removeGroup(group: Group) {
    let index = this._groups.indexOf(group, 0);
    if (index > -1) {
      this._groups.splice(index, 1);
    }

    this.initiateArrays();

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

  openModal(template: TemplateRef<any>, groupId: number, memberName?: string) {
    let group = groupId == -1 ? null : this.findGroupWithId(groupId);
    this._groupSharedService.openModal(this, this._modalService, template, group, memberName);
  }

  /**
   * Sorts the groups alphabetically
   */
  initiateArrays() {
    this._groups.sort(/*(a, b) => a.name > b.name ? 1 : -1*/);
    this._filteredGroups = this._groups;
    this._returnedArray = this._filteredGroups.slice(0, this.maxNumberOfGroupsPerPage);
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
   * When submitting the form in order to create a new group.
   */
  onSubmit(event: any) {
    // todo get group out of event rather than groupSharedService
    if (event) {
      const newGroup = this._groupSharedService.getGroup();
      this._groupsDataService.addNewGroup(this.school.id, newGroup).subscribe(value => {

        // add newly created group to list of groups.
        this._groups.push(value);
        this.initiateArrays();

        this._groupSharedService.resetGroupForm();

        // show alert (modal)
        this.add(newGroup.name);
      });
    }
  }

  private findGroupWithId(groupId: number) {
    return this._groups.find(g => g.id == groupId);
  }

  decline(): void {
    this._groupSharedService.decline();
    this.modalRef.hide();
  }

  confirm(): void {
    this._groupSharedService.confirm(this);
  }
}
