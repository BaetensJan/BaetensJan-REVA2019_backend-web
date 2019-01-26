import {Injectable} from '@angular/core';
import {Exhibitor} from "../models/exhibitor.model";

@Injectable({
  providedIn: 'root'
})
export class ExhibitorShareService {
  /**
   * Is it an edit or a create
   */
  private _aanpassen: boolean;
  /**
   * @ignore
   */
  private _exhibitor: Exhibitor;

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
   * Getter for Exhibitor
   */
  get exhibitor(): Exhibitor {
    return this._exhibitor;
    //TODO exhibitor op null zetten eens hij wordt opgehaald,
    //TODO omdat er checks zijn of deze methode niet undefined teruggeeft.
  }

  /**
   * Setter for Exhibitor
   *
   * @param value
   */
  set exhibitor(value: Exhibitor) {
    this._exhibitor = value;
  }
}
