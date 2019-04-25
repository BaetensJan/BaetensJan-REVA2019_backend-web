import {Component, OnInit, TemplateRef} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {Group} from "../../models/group.model";
import {GroupsDataService} from "../../groups/groups-data.service";
import {BsModalRef, BsModalService} from "ngx-bootstrap";

@Component({
  selector: 'app-assignment-detail',
  templateUrl: './assignment-detail.component.html',
  styleUrls: ['./assignment-detail.component.css']
})
export class AssignmentDetailComponent implements OnInit {

  private sub: any;
  public group: Group;
  public groupId: number;
  public selectedPhotoUrl: string;
  modalRef: BsModalRef; // modal that appears, asking for confirmation to remove a member from a group.

  constructor(
    private _modalService: BsModalService,
    private router?: Router,
    private route?: ActivatedRoute,
    private _groupsDataService?: GroupsDataService,
  ) {
  }

  ngOnInit() {
    this.sub = this.route
      .queryParams
      .subscribe(params => {
        // Defaults to 0 if no query param provided.
        this.groupId = params['groupId'] || 0;
        // todo make method that returns boolean if group with groupId exists in database
        // else, go to home -> this.router.navigate(["/assignmentdetail"], {queryParams: {groupId: group.id}});
        this._groupsDataService.getGroup(this.groupId).subscribe(value => {
          if (!value) this.router.navigate(["/"]);
          else this.group = value;
        });
      });
  }

  BackToAssignment() {
    this.router.navigate(["/opdrachten"]);
  }

  hideQuestion(questionText: string) {
    return questionText.includes("foto");
  }

  // goToBlankPageWithPhoto(photoUrl: string){
  //   this.router.navigate(['/opdracht-foto'], {queryParams: {photoId: photoUrl}});
  // }

  /** MODAL / POPUP **/
  openPhotoModal(template: TemplateRef<any>, assignmentPhotoUrl: string) {
    this.selectedPhotoUrl = assignmentPhotoUrl;
    this.modalRef = this._modalService.show(template, {class: 'modal-sm'});
  };
}
