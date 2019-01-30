import {AfterViewInit, ChangeDetectorRef, Component} from "@angular/core";
import {Category} from "../models/category.model";
import {Router} from "@angular/router";
import {CategoryShareService} from "./category-share.service";
import {CategoriesDataService} from "../categories/categories-data.service";

@Component({
  selector: 'app-category',
  templateUrl: './category.component.html',
  styleUrls: ['./category.component.css']
})
export class CategoryComponent implements AfterViewInit {
  /**
   * @ignore
   */
  public selectedCategorie: Category;
  /**
   * @ignore
   */
  public heeftWaarde = false;
  /**
   * @ignore
   */
  public aanpassen: boolean;

  /**
   * Constructor
   *
   * @param router
   * @param _categorieShareService
   * @param _categorieenDataSevice
   * @param cdRef
   */
  constructor(
    private router: Router,
    private _categorieShareService: CategoryShareService,
    private _categorieenDataSevice: CategoriesDataService,
    private cdRef: ChangeDetectorRef
  ) {
  }

  /**
   * Loads category or creates empty category based on editing or creating
   */
  ngAfterViewInit() {
    this.selectedCategorie = this._categorieShareService.category;
    this.aanpassen = this._categorieShareService.aanpassen;
    if (this.selectedCategorie == undefined) {
      this.selectedCategorie = new Category();
      this.selectedCategorie.name = "";
      this.selectedCategorie.description = "";
    }
    this.heeftWaarde = true;
    this.cdRef.detectChanges();
  }

  /**
   * Click event to save or create Category
   */
  onOpslaan() {
    if (this.selectedCategorie.id == undefined) {
      this._categorieenDataSevice.voegCategorieToe(this.selectedCategorie).subscribe(categorie => {
        this.router.navigate(["/categorieen"]);
      });
      setTimeout(()=>{    //<<<---    using ()=> syntax
        this.router.navigate(["/categorieen"]);
      }, 3000);

    } else {
      this._categorieenDataSevice.updateCategorie(this.selectedCategorie).subscribe(categorie => {
        this.router.navigate(["/categorieen"]);
      });
    }
  }
}
