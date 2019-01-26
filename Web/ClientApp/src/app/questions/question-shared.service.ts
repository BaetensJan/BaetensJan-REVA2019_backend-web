import { Injectable } from '@angular/core';
import {Question} from "../models/question.model";

@Injectable({
  providedIn: 'root'
})
export class QuestionSharedService {

  /**
   * Is it an edit or a create
   */
  private _edit: boolean;
  /**
   * @ignore
   */
  private _question: Question;

  /**
   * Getter for Edit
   */
  get edit(): boolean {
    return this._edit;
  }

  /**
   * Setter for Edit
   *
   * @param value
   */
  set edit(value: boolean) {
    this._edit = value;
  }

  /**
   * Getter for Question
   */
  get question(): Question {
    return this._question;
    //TODO question op null zetten eens hij wordt opgehaald,
    //TODO omdat er checks zijn of deze methode niet undefined teruggeeft.
  }

  /**
   * Setter for Question
   *
   * @param value
   */
  set question(value: Question) {
    this._question = value;
  }
}
