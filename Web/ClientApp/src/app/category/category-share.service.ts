import {Injectable} from "@angular/core";
import {Category} from "../models/category.model";

@Injectable({
  providedIn: 'root'
})
export class CategoryShareService {
  /**
   * Is it an edit or a create
   */
  private _aanpassen: boolean;
  /**
   * @ignore
   */
  private _category: Category;

  /**
   * Getter for Edit
   */
  get aanpassen(): boolean {
    return this._aanpassen;
  }

  /**
   * Setter for Edit
   *
   * @param value
   */
  set aanpassen(value: boolean) {
    this._aanpassen = value;
  }

  /**
   * Getter for Category
   */
  get category(): Category {
    return this._category;
  }

  /**
   * Setter for Category
   *
   * @param value
   */
  set category(value: Category) {
    this._category = value;
  }
}
