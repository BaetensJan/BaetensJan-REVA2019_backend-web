import { Component, OnInit } from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {Group} from "../models/group.model";
import {Observable} from "rxjs";
import {GroupsDataService} from "../groups/groups-data.service";

@Component({
  selector: 'app-assignment-detail',
  templateUrl: './assignment-detail.component.html',
  styleUrls: ['./assignment-detail.component.css']
})
export class AssignmentDetailComponent implements OnInit {

  private sub: any;
  public group: Group;
  public groupId: number;

  constructor(private router: Router,private route: ActivatedRoute, private _groupsDataService: GroupsDataService
              /*, private _assignmentDataService: AssignmentDataService*/) { }

  ngOnInit() {
    this.sub = this.route
      .queryParams
      .subscribe(params => {
        // Defaults to 0 if no query param provided.
        this.groupId = params['groupId'] || 0;
        this._groupsDataService.getGroup(this.groupId).subscribe(value => {
          this.group = value;
        });
      });
  }

  BackToAssignment() {
    this.router.navigate(["/opdrachten"]);
  }
}
