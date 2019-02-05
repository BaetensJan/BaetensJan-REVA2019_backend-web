import {Component, OnInit, TemplateRef} from '@angular/core';
import {InvitationService} from "../invitation.service";
import {TeacherRequest} from "../../../models/teacherRequest.model";
import {BsModalRef, BsModalService} from "ngx-bootstrap";

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

  constructor(private _invitationService: InvitationService, private modalService: BsModalService) {
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
        let index = this.requests.indexOf(this.requests.find((c) => c.id === this.clickedItem.id));
        this.requests.splice(index, 1);
      });
    } else {
      this._invitationService.deleteTeacher(this.clickedItem.id).subscribe(categorie => {
        let index = this.requests.indexOf(this.requests.find((c) => c.id === this.clickedItem.id));
        this.requests.splice(index, 1);
      });
    }
    this.clickedItem = null;
    this.refModal.hide();
    // window.location.reload();
  }

  get requests(): TeacherRequest[] {
    return this._requests;
  }
}
