import {Component, ElementRef, TemplateRef, ViewChild} from '@angular/core';
import {CategoriesDataService} from "./categories-data.service";
import {Category} from "../models/category.model";
import {BsModalRef, BsModalService} from "ngx-bootstrap";
import {Router} from "@angular/router";
import {CategoryShareService} from "../category/category-share.service";
import {Exhibitor} from "../models/exhibitor.model";

@Component({
  selector: 'app-categories',
  templateUrl: './categories.component.html',
  styleUrls: ['./categories.component.css'],
})
export class CategoriesComponent {

  /**
   * @ignore
   */
  data: Category[];
  /**
   * @ignore
   */
  clickedItem: Category;
  /**
   * @ignore
   */
  refModal: BsModalRef;

  /**
   * Constructor
   *
   * @param router: Router
   * @param modalService: BsModalService
   * @param _categorieenDataService: CategorieenDataService
   * @param _categorieShareService: CategorieShareService
   */
  constructor(private router: Router,
              private modalService: BsModalService,
              private _categorieenDataService: CategoriesDataService,
              private _categorieShareService: CategoryShareService
  ) {
    this.categorieen();
  }

  /**
   * Haalt categorieën uit de backend en geeft dit door naar de datasource
   */
  categorieen() {
    this._categorieenDataService.categories.subscribe(categories => {
      this.data = categories;
    });
  }

  /**
   * Click event om categorie toe te voegen. Geeft een lege categorie door aan share service om zo de data te kunnen verkrijgen in het categorie component.
   *
   */
  onToevoegenCategorie() {
    this._categorieShareService.category = null;
    this.router.navigate((["/categorie"]))
  }

  /**
   * Click event om categorie te verwijderen. Geeft de categorie door aan de modal voor bevestiging.
   *
   * @param row: Categorie
   */
  openModal(template: TemplateRef<any>, cat: Category) {
    this.clickedItem = cat;
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
   * Click event om categorie aan te passen. Geeft de categorie door aan share service om zo de data te kunnen verkrijgen in het categorie component.
   *
   * @param row: Categorie
   */
  onAanpassenCategorie(row: Category) {
    this._categorieShareService.category = row;
    this._categorieShareService.aanpassen = true;
    this.router.navigate(["/categorie"]);
  }

  /**
   * Bevestiging van het verwijderen van een categorie. Dit wordt gebruikt bij het modal om te bevestigen.
   *
   */
  verwijderenBevestigd() {
    console.log("verwijderen van " + this.clickedItem.name);
    this.refModal.hide();
    this._categorieenDataService.verwijderCategorie(this.clickedItem).subscribe(categorie => {
      let index = this.data.indexOf(this.data.find((c) => c.id === categorie.id));
      this.data.splice(index, 1);
    });
    window.location.reload();
  }
}
