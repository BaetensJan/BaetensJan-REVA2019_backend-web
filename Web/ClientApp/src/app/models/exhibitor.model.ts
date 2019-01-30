import {Category} from "./category.model";
import {Question} from "./question.model";

export class Exhibitor {
  /**
   * @ignore
   * Id is of type number because we have integers as id
   * in the dotnet backend and database.
   */
  private _id: number;
  private _exhibitorNumber:string; // exhibition number - Standnummer
  /**
   * @ignore
   */
  private _name: string;
  /**
   * @ignore
   */
  private _categories: Category[];
  /**
   * @ignore
   */
  private _x: number;
  /**
   * @ignore
   */
  private _y: number;

  constructor() {
    this._name = "";
    // entrance position
    this._x = 0.0966804979253112;
    this._y = 0.612603305785124;
    this.categories = [];
    this._exhibitorNumber = "";
  }

  /**
   * Static JSON to Object parser
   * @param json
   */
  static fromJSON(json: any): Exhibitor {
    const rec = new Exhibitor();
    console.log(json);
    rec._name = json.name;
    rec._exhibitorNumber = json.exhibitorNumber;
    //rec._categories = json.forEach(c => c.category = Category.fromJSON(c.category));
    rec._categories = json.categories.map(ce=>ce.category);
    rec._id = json.id;
    rec._y = json.y;
    rec._x = json.x;
    return rec;
  }

  /**
   * Object to JSON parser
   */
  toJSON() {
    return {
      id: this._id,
      name: this._name,
      exhibitorNumber: this._exhibitorNumber,
      categories: this._categories,
      x: this._x,
      y: this._y
    };
  }

  /**
   * Getter for Id
   */
  get id(): number {
    return this._id;
  }


  /**
   * Setter for Id
   *
   * @param value
   */
  set id(value: number) {
    this._id = value;
  }

  /**
   * Getter for Name
   */
  get name(): string {
    return this._name;
  }

  /**
   * Setter for Name
   *
   * @param value
   */
  set name(value: string) {
    this._name = value;
  }
  get exhibitorNumber():string{
    return this._exhibitorNumber;
  }
  set exhibitorNumber(value:string){
    this._exhibitorNumber = value;
  }
  /**
   * Getter for Category
   */
  get categories(): Category[] {
    return this._categories;
  }

  /**
   * Setter for Category
   *
   * @param value
   */
  set categories(value: Category[]) {
    this._categories = value;
  }

  /**
   * Getter for X
   */
  get x(): number {
    return this._x;
  }

  /**
   * Setter for X
   * @param value
   */
  set x(value: number) {
    this._x = value;
  }

  /**
   * Getter for Y
   */
  get y(): number {
    return this._y;
  }

  /**
   * Setter for Y
   * @param value
   */
  set y(value: number) {
    this._y = value;
  }

  get getCategoriesAsString() {
    let categoriesString = this._categories[0].name;
    if (this._categories.length > 1) {
      for (let i = 1; i < this._categories.length; i++) {
        categoriesString += ", " + this._categories[i].name;
      }
    }
    return categoriesString;
  }
}
