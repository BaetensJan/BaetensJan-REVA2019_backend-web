import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {map} from "rxjs/operators";
import {Category} from "../models/category.model";
import {Exhibitor} from "../models/exhibitor.model";

@Injectable({
  providedIn: 'root'
})
export class CategoriesDataService {
  /**
   * Base url of the to connected connection
   */
  private readonly _appUrl = '/API/Category';

  /**
   * Constructor
   *
   * @param http
   */
  constructor(private http: HttpClient) {
  }

  /**
   * Makes call to backend and returns Categories
   */
  get categories(): Observable<Category[]> {
    return this.http
      .get(`${this._appUrl}/Categories/`)
      .pipe(map((list: any[]): Category[] => list.map(Category.fromJSON)));
  }

  /**
   * Makes call to backend and returns Category
   */
  categorieByName(categoryName: string): Observable<Category> {
    return this.http
      .get(`${this._appUrl}/CategorieByName/${categoryName}`)
      .pipe(map(Category.fromJSON));
  }

  /**
   * Makes call to backend and adds a Category
   *
   * @param categorie
   */
  voegCategorieToe(categorie: Category): Observable<Category> {
    return this.http
      .post(`${this._appUrl}/AddCategory/`, categorie)
      .pipe(map(Category.fromJSON));
  }

  /**
   * Makes call to backend and removes Category
   *
   * @param rec
   */
  verwijderCategorie(rec: Category): Observable<Category> {
    return this.http
      .delete(`${this._appUrl}/RemoveCategory/${rec.id}`)
      .pipe(map(Category.fromJSON));
  }

  /**
   * Makes call to the backend and removes all the exhibitors
   * @param rec
   */
  removeCategories(): Observable<Category[]> {
    return this.http
      .delete(`${this._appUrl}/RemoveCategories`)
      .pipe(map((list: any[]): Category[] => list.map(Category.fromJSON)));
  }

  /**
   * Makes call to backend and updates an existing Category
   *
   * @param rec
   */
  updateCategorie(rec: Category): Observable<Category> {
    return this.http
      .put(`${this._appUrl}/UpdateCategory/${rec.id}`, rec)
      .pipe(map(Category.fromJSON));
  }
}
