import {Component, OnInit, TemplateRef} from '@angular/core';
import {InvitationService} from "../invitation.service";
import {BsModalRef, BsModalService} from "ngx-bootstrap";
import {TeacherRequest} from "../../models/teacherRequest.model";
import {Router} from "@angular/router";
import {RequestsShareService} from "./requests-share.service";

@Component({
  selector: 'app-requests',
  templateUrl: './requests.component.html',
  styleUrls: ['./requests.component.css']
})
export class RequestsComponent implements OnInit {

  private _requests: TeacherRequest[];
  /**
   * @ignore
   */
  clickedItem: TeacherRequest;
  /**
   * @ignore
   */
  refModal: BsModalRef;
  modalBody: string;
  accept: boolean;

  constructor(private router: Router,
              private _invitationService: InvitationService,
              private modalService: BsModalService,
              private _requestTeacherShareService: RequestsShareService
  ) {
  }

  ngOnInit() {
    this._invitationService.teacherRequests.subscribe(value =>
      this._requests = value);
  }

  /**
   * Click event om categorie te verwijderen. Geeft de categorie door aan de modal voor bevestiging.
   *
   * @param row: Categorie
   */
  openModal(template: TemplateRef<any>, tr: TeacherRequest, accept: boolean) {
    this.clickedItem = tr;
    this.accept = accept;
    this.modalBody = `Bent u zeker dat u de aanvraag voor school ${tr.schoolName} wil ${accept ? "aanvaarden" : "verwijderen"}?`;

    this.refModal = this.modalService.show(template, {class: 'modal-sm'});
  }

  /**
   * Toggle voor modal te tonen.
   *
   */
  hideModal() {
    this.refModal.hide();
  }

  /**
   * Clicked on confirm in modal
   *
   */
  modalAccepted() {
    if (this.accept) {
      this._invitationService.createTeacher(this.clickedItem.id).subscribe(request => {
        this.removeRequest();
      });
    } else {
      this._invitationService.deleteTeacher(this.clickedItem.id).subscribe(request => {
        this.removeRequest();
      });
    }
    this.refModal.hide();
  }

  onToevoegenAanvraag() {
    this._requestTeacherShareService.teacherRequest = null;
    this.router.navigate(["invite-request"]);
  }

  onAanpassenRequest(request: TeacherRequest) {
    console.log(request);
    this._requestTeacherShareService.teacherRequest = request;
    this._requestTeacherShareService.aanpassen = true;
    this.router.navigate(["invite-request"]);
  }

  /**
   * Removes a request from the requests attribute (NOT DB)
   */
  private removeRequest(): void {
    console.log(this.requests);
    let index = this.requests.indexOf(this.requests.find((request: TeacherRequest) => request.id === this.clickedItem.id));
    this.requests.splice(index, 1);
    console.log(this.requests);
    this.clickedItem = null;
  }

  get requests(): TeacherRequest[] {
    return this._requests;
  }
}
