import {Exhibitor} from "./exhibitor.model";
import {Question} from "./question.model";

export class Assignment {
  /**
   * @ignore
   */
  private _question: Question;
  /**
   * @ignore
   */
  private _answer: string;
  /**
   * @ignore
   */
  private _notes: string;
  /**
   * @ignore
   */
  private _extra: boolean;
  /**
   * @ignore
   */
  private _photo: string;
  /**
   * @ignore
   */
  private _exhibitor: Exhibitor;

  /**
   * Constructor
   *
   * @param question
   * @param answer
   * @param notes
   * @param photo
   * @param exhibitor
   * @param extra
   */
  constructor(
    question: Question,
    answer: string,
    notes: string,
    photo: string,
    exhibitor: Exhibitor,
    extra: boolean,
  ) {
    this._question = question;
    this._answer = answer;
    this._notes = notes;
    this._photo = photo;
    this._exhibitor = exhibitor;
    this._extra = extra;
  }

  /**
   * Static JSON to Object parser
   *
   * @param json
   */
  static fromJSON(json: any): Assignment {
    const rec = new Assignment(
      json.question,
      json.answer,
      json.notes,
      json.photo,
      json.exhibitor,
      json.extra
    );
    return rec;
  }

  /**
   * Object to JSON parser
   */
  toJSON() {
    return {
      question: this._question,
      answer: this._answer,
      notes: this._notes,
      exhibitor: this._exhibitor,
      photo: this._photo,
      extra: this._extra,
    };
  }

  /**
   * Getter Question
   */
  get question(): Question {
    return this._question;
  }

  set question(q) {
    this._question = q;
  }
  /**
   * Getter for Answer
   */
  get answer(): string {
    return this._answer;
  }

  /**
   * Getter for Notes
   */
  get notes(): string {
    return this._notes;
  }

  /**
   * Getter for Exhibitor
   */
  get exhibitor(): Exhibitor {
    return this._exhibitor;
  }

  /**
   * Getter for Photo
   */
  get photo(): string {
    return this._photo;
  }
  /**
   * Getter for Extra
   */
  get extra(): boolean {
    return this._extra;
  }
}
