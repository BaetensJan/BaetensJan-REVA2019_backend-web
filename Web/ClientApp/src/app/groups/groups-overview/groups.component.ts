import {Component, TemplateRef} from '@angular/core';
import {GroupsDataService} from "../groups-data.service";
import {Group} from "../../models/group.model";
import {School} from "../../models/school.model";
import {PageChangedEvent} from 'ngx-bootstrap/pagination';
import {AbstractControl, FormBuilder, FormGroup, Validators} from "@angular/forms";
import {AlertComponent, BsModalRef, BsModalService} from "ngx-bootstrap";
import {AppShareService} from "../../app-share.service";
import {SchoolDataService} from "../../schools/school-data.service";
import {GroupSharedService} from "../group-shared.service";
import {Router} from "@angular/router";
import {AuthenticationService} from "../../user/authentication.service";
import {AssignmentDataService} from "../../assignment/assignment-data.service";
import {Observable, of as observableOf} from "rxjs";
import {map} from "rxjs/operators";
import {InviteRequestComponent} from "../../invitation/send-or-update-request/invite-request.component";

@Component({
  selector: 'groups',
  templateUrl: './groups.component.html',
  styleUrls: ['./groups.component.css'],
  animations: []
})
export class GroupsComponent {
  alerts: any[] = [];

  modalRef: BsModalRef; // modal that appears, asking for confirmation to remove a member from a group.
  modalMessage: string;

  private _schoolLoginNameForm: FormGroup;
  get schoolLoginNameForm() {
    return this._schoolLoginNameForm;
  }

  private _createGroup: boolean = false;
  get createGroup(): boolean {
    return this._createGroup;
  }

  get groupForm(): FormGroup {
    return this._groupSharedService.groupForm;
  }

  private _isAdmin: string;
  get isAdmin(): boolean {
    return this._isAdmin == 'True';
  }

  currentPage; // the current page in the pagination (e.g. page 1, 2 ...)
  private _groups: Group[]; // array containing the groups that fits the filter.
  get groups(): Group[] {
    return this._groups;
  }

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
   * @param _fb
   * @param _modalService
   * @param _appShareService
   * @param _authService
   */
  constructor(private router: Router,
              private _groupsDataService: GroupsDataService,
              private _groupSharedService: GroupSharedService,
              private _schoolDataService: SchoolDataService,
              private _assignmentDataService: AssignmentDataService,
              private _fb: FormBuilder,
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
              group.schoolLoginName = school.loginName;

              this._groups.push(group);
            }
          }
          this.initiateArrays();
        });
      });
    } else {
      this._schoolDataService.getSchool(schoolId).subscribe((school: School) => {
        this._school = school;

        this._schoolLoginNameForm = this._fb.group({
          schoolLoginName: [
            this._school.loginName,
            [
              Validators.required,
              Validators.minLength(2),
              Validators.maxLength(15),
              GroupSharedService.schoolNamePatternValidator,
              GroupSharedService.noWhitespaceValidator
            ],
            this.serverSideValidateSchoolLoginName(),
          ],
        });

        this._groups = this._school.groups;

        this.initiateArrays();
      });
    }
    this._assignmentDataService.getApplicationStartDate().subscribe(
      value => this._applicationStartDate = new Date(value));
  }

  public createGroupClicked() {
    this._createGroup = !this._createGroup;
  }

  public updateGroup(group: Group) {
    this._groupSharedService.updateGroup(this._school.id, group);
    this.router.navigate(["/group/updateGroup"]);
  }

  private removeGroup() {
    this._groupsDataService.removeGroup(this._groupSharedService.groupToRemove).subscribe(value => {

      let index = this._groups.indexOf(this._groupSharedService.groupToRemove, 0);
      if (index > -1) {
        this._groups.splice(index, 1);
      }

      this.initiateArrays();

      this._groupSharedService.groupToRemove = null;

      this.modalRef.hide();
    });
  }

  /** FILTER **/
  public filter(token: string) {
    token = token.toLowerCase();
    if (!token) {
      this._filteredGroups = this._groups;
    } else {
      this._filteredGroups = [];
      this._groups.forEach((group: Group) => {
        if (group.name.toLowerCase().includes(token)) {
          this._filteredGroups.push(group);
        } else {
          for (let i in group.members) {
            let member = group.members[i];
            if (member.toLowerCase().includes(token)) {
              this._filteredGroups.push(group);
              break;
            }
          }
        }
      });
    }
    this._returnedArray = this._filteredGroups.slice(0, this.maxNumberOfGroupsPerPage);
    this.currentPage = 1; // switches current page in pagination back to page 1
  }

  openModalUpdateLoginName(template: TemplateRef<any>) {
    const message = `Door het wijzigen van de school login zullen ook groepjes in de app moeten
    inloggen met deze nieuwe naam. Bijvoorbeeld nu: '${this._school.loginName}.groep1' wordt na de wijziging:
     '${this._schoolLoginNameForm.get("schoolLoginName").value}.groep1' (breng de groepen hiervan op de hoogte!).
     Ben je zeker dat je de naamswijziging wil doorvoeren?`;

    this.openModal(template, message);
  }

  openModalRemoveMember(template: TemplateRef<any>, removeGroup: boolean) {
    const message = `Ben je zeker dat ${this._groupSharedService.memberToRemove}
     verwijderd mag worden uit de groep?`;

    this.openModal(template, message);
  }

  openModalRemoveGroup(template: TemplateRef<any>, groupId: number) {
    const group = this.findGroupWithId(groupId);
    this._groupSharedService.groupToRemove = group;

    const message = `Ben je zeker dat de groep met groepsnaam ${group.name} verwijderd mag worden? 
      De ingediende opdrachten van deze groep worden hierdoor ook verwijderd.`;

    this.openModal(template, message);
  }

  /** MODAL / POPUP **/
  openModal(template: TemplateRef<any>, modalMessage: string) {
    this.modalMessage = modalMessage;
    this.modalRef = this._modalService.show(template, {class: 'modal-sm'});
  };

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
    this._schoolLoginNameForm.get("schoolLoginName").setValue(this._school.loginName);
    this.modalRef.hide();
  }

  confirm(): void {
    if (this._groupSharedService.groupToRemove != null) {
      this.removeGroup()
    } else if (this._groupSharedService.memberToRemove != '') {
      this._groupSharedService.confirm();
    } else {
      this.updateSchoolLoginName();
    }
    this.modalRef.hide();
  }

  updateSchoolLoginName() {
    const newSchoolLoginName = this._schoolLoginNameForm.get("schoolLoginName").value;

    this._schoolDataService.updateSchoolLoginName(this._school.id, newSchoolLoginName).subscribe(_ => {
      this._school.loginName = newSchoolLoginName;
      this.add();
    });
  }

  /**
   * Shows a message when a group was successfully created.
   * @param groupName
   */
  public add(groupName?: string): void {
    const message = groupName ? `De nieuwe groep met groepsnaam '${groupName}' werd succesvol toegevoegd.`
      : `Login van school succesvol veranderd naar '${this._school.loginName}'. (om: ${new Date().toLocaleTimeString()})`;

    this.alerts.push(
      {
        type: 'success',
        msg: message,
        timeout: 5000
      });
  }

  onClosed(dismissedAlert: AlertComponent): void {
    this.alerts = this.alerts.filter(alert => alert !== dismissedAlert);
  }

  /**
   * Checks if schoolName already exists in database
   */
  serverSideValidateSchoolLoginName(): (control: AbstractControl) => Observable<{ [p: string]: any }> {
    return (control: AbstractControl): Observable<{ [key: string]: any }> => {

      if (control.value == this._school.loginName) {
        return observableOf(null);
      }

      return this._schoolDataService
        .checkLoginNameAvailability(control.value)
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
}
