import {Component, OnInit, TemplateRef} from '@angular/core';
import {BsModalRef, BsModalService, TypeaheadMatch} from "ngx-bootstrap";
import {Router} from "@angular/router";
import {ExhibitorsDataService} from "./exhibitors-data.service";
import {ExhibitorShareService} from "../exhibitor/exhibitor-share.service";
import {Exhibitor} from "../models/exhibitor.model";
import {Group} from "../models/group.model";
import {AppShareService} from "../AppShareService";

@Component({
  selector: 'app-exhibitors',
  templateUrl: './exhibitors.component.html',
  styleUrls: ['./exhibitors.component.css']
})
export class ExhibitorsComponent implements OnInit {
  /**
   * @ignore
   */
  private _exhibitors: Exhibitor[];
  /**
   * @ignore
   */
  private _contentArray: Exhibitor[];
  /**
   * @ignore
   */
  clickedItem: Exhibitor;
  /**
   * @ignore
   */
  refModal: BsModalRef;

  _filteredExhibitors: Exhibitor[];
  filterValue: string = "";
  successMessage = "";
  showMessage = false;
  showMessageFail = false;
  teller = 0;
  tellertotaal = 0;

  /**
   * Constructor
   *
   * @param router: Router
   * @param modalService: BsModalService
   * @param _exhibitorsDataService: ExhibitorsDataService
   * @param _exhibitorShareService: ExhibitorsShareService
   */
  ngOnInit() {
    this._exhibitorsDataService.exhibitors.subscribe(exhibitors => {
      for(let exhib of exhibitors){
        console.log(exhib);
        if(exhib.x == 0 && exhib.y == 0) {
          this.teller++;
        }
      }
      console.log(this.teller);
      this.tellertotaal = exhibitors.length;
      if(this.teller > 0) {
        this.showMessage = true;
        this.successMessage = "U heeft nog "+this.teller +" van de "+this.tellertotaal +" exposanten niet aangeduid op de kaart.";
      }
    });
    this.filterValue = "";
    this._exhibitorsDataService.exhibitors.subscribe(exhibitors => {
      this._exhibitors = exhibitors.sort((a, b) => a.name > b.name ? 1 : -1);
      this._contentArray = this._exhibitors;
      this._filteredExhibitors = this._exhibitors
    });
  }

  constructor(private router: Router,
              private modalService: BsModalService,
              private _appShareService: AppShareService,
              private _exhibitorsDataService: ExhibitorsDataService,
              private _exhibitorShareService: ExhibitorShareService
  ) {
  }


  public filter(token: string) {
    this._filteredExhibitors = this._exhibitors.filter((exhibitor: Exhibitor) => {
      return exhibitor.name.toLowerCase().startsWith(token.toLowerCase());

      //return question.categoryExhibitor.exhibitor.name.toLowerCase().startsWith(token.toLowerCase()) ||
      //  question.categoryExhibitor.category.name.toLowerCase().startsWith(token.toLowerCase());
    });
  }

  /**
   * On enter of click on typeAHeadMatch, the chosen exhibitor will be displayed.
   * @param event
   */
  onSelect(event: TypeaheadMatch): void {
    let exhibitor = event.item;
    this._contentArray = [];
    this._contentArray.push(exhibitor);
  }

  /**
   * No result matching the searchterm in the filter.
   * @param event
   */
  typeaheadNoResults(event: boolean): void {
    this._contentArray = this._exhibitors;
  }
  /**
   * Returns the filtered exhibitors
   *
   */
  get exhibitors() {
    return this._contentArray;
  }

  /**
   * Toggle voor modal te tonen.
   *
   */
  hideModal() {
    this.refModal.hide();
  }

  /**
   * Click event om exhibitor toe te voegen. Geeft een lege exhibitor door aan share service om zo de data te kunnen verkrijgen in het exhibitor component.
   *
   */
  onToevoegenExhibitor() {
    this._exhibitorShareService.exhibitor = null;
    this.router.navigate((["/exposant"]))
  }

  /**
   * Click event om exhibitor aan te passen. Geeft de exhibitor door aan share service om zo de data te kunnen verkrijgen in het exhibitor component.
   *
   */
  onAanpassenExhibitor(row: Exhibitor) {
    this._exhibitorShareService.exhibitor = row;
    this._exhibitorShareService.aanpassen = true;
    this.router.navigate(["/exposant"]);
  }

  /**
   * Bevestiging van het verwijderen van een exhibitor. Dit wordt gebruikt bij het modal om te bevestigen.
   *
   */
  verwijderenBevestigd() {
    this.refModal.hide();
    this._exhibitorsDataService.verwijderExhibitor(this.clickedItem).subscribe(exhibitor => {
      let index = this._exhibitors.indexOf(this._exhibitors.find((c) => c.id === exhibitor.id));
      this._exhibitors.splice(index, 1);
      index = this._contentArray.indexOf(this._contentArray.find((c) => c.id === exhibitor.id));
      this._contentArray.splice(index, 1);
    });
  }

  openModal(template: TemplateRef<any>, exhibitor: Exhibitor) {
    this.clickedItem = exhibitor;
    this.refModal = this.modalService.show(template, {class: 'modal-sm'});
  }
}
