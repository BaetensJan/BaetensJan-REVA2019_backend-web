import {Component, OnInit} from '@angular/core';
import {GroupSharedService} from "../group-shared.service";
import {Group} from "../../models/group.model";
import {AbstractControl, FormBuilder, FormGroup, Validators} from "@angular/forms";
import {GroupsDataService} from "../groups-data.service";
import {Observable} from "rxjs";
import {map} from "rxjs/operators";
import {Router} from "@angular/router";
import { of } from 'rxjs';

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

  constructor(private _groupSharedService: GroupSharedService,
              private _groupDataService: GroupsDataService,
              private fb: FormBuilder,
              private router: Router) {
  }

  ngOnInit() {
    if (this._groupSharedService.edit == false || !this._groupSharedService.group)
      this.router.navigate(["groepen"]);
    else {
      this._schoolId = this._groupSharedService.schoolId;
      this._group = this._groupSharedService.group;
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
      this.router.navigate(["groepen"]);
    });
  }

  openModal(template, member: string) {
    console.log("remove " + member);
  }

  get groupForm() {
    return this._groupForm;
  }

  get group() {
    return this._group;
  }
}
