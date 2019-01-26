import {Exhibitor} from "./exhibitor.model";

export class Category {
  /**
   * @ignore
   */
  private _id: number;
  /**
   * @ignore
   */
  private _name: string;
  /**
   * @ignore
   */
  private _description: string;

  /**
   * @ignore
   */
  private _exhibitors: Exhibitor[];

  get exhibitors(){
    return this._exhibitors;
  }

  set exhibitors(exhibitors){
    this._exhibitors = exhibitors;
  }

  /**
   * Getter for ID
   */
  get id(): number {
    return this._id;
  }

  /**
   * Setter for Id
   *
   * @param id
   */
  set id(id: number) {
    this._id = id;
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
   * @param name
   */
  set name(name: string) {
    this._name = name;
  }

  /**
   * Getter for Description
   */
  get description(): string {
    return this._description;
  }

  /**
   * Setter for Description
   *
   * @param value
   */
  set description(value: string) {
    this._description = value;
  }

  /**
   * Static JSON to Object parser
   *
   * @param json
   */
  static fromJSON(json: any): Category {
    const rec = new Category();
    rec._exhibitors = json.exhibitors.map(ce => ce.exhibitor); // from CategoryExhibitor to exhibitor
    rec._id = json.id;
    rec._name = json.name;
    rec._description = json.description;
    return rec;
  }

  /**
   * Object to JSON parser
   */
  toJSON() {
    return {
      id: this._id,
      name: this._name,
      description: this._description,
      exhibitors: this._exhibitors
    };
  }
}
