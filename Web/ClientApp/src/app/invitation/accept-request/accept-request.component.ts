import {Component, OnInit} from '@angular/core';
import {AuthenticationService} from "../../user/authentication.service";
import {ActivatedRoute, Router} from "@angular/router";
import {InvitationService} from "../invitation.service";
import {TeacherRequest} from "../../models/teacherRequest.model";

@Component({
  selector: 'app-accept-request',
  templateUrl: './accept-request.component.html',
  styleUrls: ['./accept-request.component.css']
})
export class AcceptRequestComponent implements OnInit {

  private _email: string;
  public get email() {
    return this._email;
  }

  private _password: string;
  public get password() {
    return this._password;
  }

  constructor(private _invitationService: InvitationService,
              private router: Router,
              private route?: ActivatedRoute) {
  }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      // Defaults to 0 if no query param provided.
      const requestId = params['requestId'] || -1;
      if (requestId != -1) {
        this._invitationService.teacherRequestExist(requestId).subscribe((value: any) => {
          if (value) {
            this._invitationService.createTeacher(requestId).subscribe((value: any) => {
              this._email = value.login;
              this._password = value.password;
            });
          } else {
            this.router.navigate(["requests"]);
          }
        });

      } else {
        this.router.navigate(["requests"]);
      }
    });
  }
}
