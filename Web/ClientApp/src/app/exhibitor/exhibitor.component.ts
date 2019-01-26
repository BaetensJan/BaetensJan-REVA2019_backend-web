import {AfterViewInit, ChangeDetectorRef, Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {Router} from "@angular/router";
import {Exhibitor} from "../models/exhibitor.model";
import {ExhibitorShareService} from "./exhibitor-share.service";
import {ExhibitorsDataService} from "../exhibitors/exhibitors-data.service";
import {CategoriesDataService} from "../categories/categories-data.service";
import {Category} from "../models/category.model";
import {AuthenticationService} from "../user/authentication.service";
import {AppShareService} from "../AppShareService";


@Component({
  selector: 'app-exhibitor',
  templateUrl: './exhibitor.component.html',
  styleUrls: ['./exhibitor.component.css']
})
export class ExhibitorComponent implements AfterViewInit {
  /**
   * @ignore
   */
  @ViewChild('myCanvas', {read: ElementRef}) public canvas: ElementRef;
  /**
   * @ignore
   */
  @ViewChild('myContainer', {read: ElementRef}) public container: ElementRef;
  /**
   * @ignore
   */
  public cx: CanvasRenderingContext2D;
  /**
   * @ignore
   */
  public selectedExhibitor: Exhibitor;
  /**
   * @ignore
   */
  public allCategories: Category[];
  /**
   * All the categories that belong to the exhibitor.
   */
  public selectedCategories: Category[];
  /**
   * marker image
   */
  public marker;
  /**
   * Height of marker
   */
  markerHeight;

  /**
   * Width of marker
   */
  markerWidth;

  /**
   * Routeplan of the exhibition
   */
  img;

  /**
   * Constructor
   *
   * @param router
   * @param _exhibitorShareService
   * @param _appShareService
   * @param _exhibitorsDataService
   * @param _categoriesDataService
   * @param _authService
   * @param cdRef
   */
  constructor(
    private router: Router,
    private _exhibitorShareService: ExhibitorShareService,
    private _appShareService: AppShareService,
    private _exhibitorsDataService: ExhibitorsDataService,
    private _categoriesDataService: CategoriesDataService,
    private _authService: AuthenticationService,
    private cdRef: ChangeDetectorRef
  ) {
  }

  selectCategory(cat: Category) {
    // if category wasn't selected yet.
    let index = this.selectedCategories.findIndex(c => c.id == cat.id);
    if (index < 0) this.selectedCategories.push(cat);
    else this.selectedCategories.splice(index, 1);
    console.log(this.selectedCategories);
  }

  isSelected(category) {
    let classType = 'btn-default'; // if category wasn't selected yet, make button default.
    this.selectedCategories.forEach(cat => {
      if (cat.id == category.id) {
        classType = 'btn-primary'; // if the category was selected make button primary (blue)
        return;
      }
    });
    return classType;
  }

  /**
   * loads Canvas on view init and shows the position of the exhibitor
   */
  ngAfterViewInit() {

    this._categoriesDataService.categories.subscribe(categories => {
      this.allCategories = categories;
      this.selectedExhibitor = this._exhibitorShareService.exhibitor;
      if (this.selectedExhibitor == undefined) { // user wants to create/add a new exhibitor
        console.log("creation of new exhibitor");
        this.selectedCategories = [];
        this.selectedExhibitor = new Exhibitor();
      } else {
        this.selectedCategories = this.selectedExhibitor.categories;
      }

      setTimeout(() => {
        this.cx = (<HTMLCanvasElement>this.canvas.nativeElement).getContext('2d');

        this.draw();
      }, 1);
      console.log("drawing canvas");
    });
  }

  /**
   * Reset image and draw on location of param pos
   *
   * @param pos
   */
  private draw() {
    this.img = new Image;
    this.img.src = `/images/beursplan.jpg?dummy=${new Date().getTime()}`;

    //after image has been loaded execute this:
    this.img.onload = () => {
      this.initCanvas();

      // paint initial exposant location on map.
      console.log("marker init");
      this.marker = new Image;
      this.markerWidth = 25;
      this.markerHeight = 25;

      this.marker.src = `../../assets/marker.png`;
      this._authService.user$.subscribe(value => {
        if (value == "gilles") {
          this.marker.src = "https://bit.ly/2OOOYex";
          this.markerWidth = 50;
          this.markerHeight = 150;
        }
      });

      this.marker.onload = () => {
        this.paintMarker(this.selectedExhibitor.x * this.canvas.nativeElement.width,
          this.selectedExhibitor.y * this.canvas.nativeElement.height);
      };
    };
  }

  private initCanvas() {
    //x = scale element for the canvas (canvas resizes after screen resize & click event on canvas)
    let x = (<HTMLElement>this.container.nativeElement).clientWidth / this.img.width; //TODO met window.innerWidth werken
    this.canvas.nativeElement.width = this.img.width * x;
    this.canvas.nativeElement.height = this.img.height * x;
    //draw image on canvas
    this.cx.drawImage(this.img, 0, 0, this.img.width * x, this.img.height * x);
    this.cx.beginPath();
  }

  /**
   * To change position of exhibitor on canvas
   * @param e
   */
  private paintMarker(xPos, yPos) {
    this.initCanvas();

    this.selectedExhibitor.x = (xPos / this.canvas.nativeElement.width);
    this.selectedExhibitor.y = (yPos / this.canvas.nativeElement.height);
    this.cx.drawImage(this.marker, xPos - this.markerWidth / 2, yPos - this.markerHeight, this.markerWidth, this.markerHeight);
    this.cdRef.detectChanges();
  }

  /**
   * get mouse position compared to canvas and client screen
   *
   * @param canvas
   * @param evt
   */
  onCanvasClick(evt) {
    // getMousePos
    let rect = (<HTMLCanvasElement>this.canvas.nativeElement).getBoundingClientRect();
    this.paintMarker(evt.clientX - rect.left, evt.clientY - rect.top);
    this._appShareService.addAlert({
      type: 'success',
      msg: `${this.selectedExhibitor.name ? this.selectedExhibitor.name : "De exposant"} heeft nieuwe x coördinaat
       ${this.selectedExhibitor.x} en y coördinaat ${this.selectedExhibitor.y}}`,
      timeout: 5000
    });
  }

  /**
   * Click event to cancel
   */
  onAnnuleer() {
    this.router.navigate(["/exposanten"]);
  }

  /**
   * Click event to save/create an exhibitor
   */
  onOpslaan() {
    this.selectedExhibitor.categories = this.selectedCategories;
    if (this.selectedExhibitor.id == undefined) {
      this._exhibitorsDataService.voegExhibitorToe(this.selectedExhibitor).subscribe(exposant => {
        this.router.navigate(["/exposanten"]);
      });
    } else {
      console.log(this.selectedExhibitor);
      this._exhibitorsDataService.updateExhibitor(this.selectedExhibitor).subscribe(e => {
        this.router.navigate(["/exposanten"]);
      });

    }
  }
}
