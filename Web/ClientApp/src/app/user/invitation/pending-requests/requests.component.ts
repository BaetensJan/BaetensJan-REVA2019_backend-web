import { Component, OnInit } from '@angular/core';
import {InvitationService} from "../invitation.service";
import {TeacherRequest} from "../../../models/teacherRequest.model";

@Component({
  selector: 'app-requests',
  templateUrl: './requests.component.html',
  styleUrls: ['./requests.component.css']
})
export class RequestsComponent implements OnInit {

  private _requests: TeacherRequest[];

  constructor(private _invitationService: InvitationService) { }

  ngOnInit() {
      this._invitationService.teacherRequests.subscribe(value =>
      this._requests = value);
  }

  get requests(): TeacherRequest[]{
    return this._requests;
  }
}
