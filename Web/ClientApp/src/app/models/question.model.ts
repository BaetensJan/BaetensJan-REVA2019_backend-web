import {Exhibitor} from "./exhibitor.model";
import {Category} from "./category.model";

export class Question {
  /**
   * @ignore
   */
  private _id: number;
  /**
   * @ignore
   */
  private _questionText: string;
  private _answer: string;
  private _category: Category;
  private _exhibitor: Exhibitor;

  /**
   * Constructor
   *
   * @param question
   * @param answer
   * @param exhibitor
   * @param category
   */
  constructor(question: string, answer: string, category: Category, exhibitor: Exhibitor) {
    this._questionText = question;
    this._answer = answer;
    this._category = category;
    this._exhibitor = exhibitor;
  }

  /**
   * Static JSON to Object parser
   *
   * @param json
   */
  static fromJSON(json: any): Question {
    const rec = new Question(json.questionText, json.answer, json.categoryExhibitor.category, json.categoryExhibitor.exhibitor);
    rec._id = json.id;
    return rec;
  }

  /**
   * Object to JSON converter
   */
  toJSON() {
    return {
      id: this._id,
      questionText: this._questionText,
      answer: this._answer,
      category: this._category,
      exhibitor: this._exhibitor
    };
  }

  /**
   * Checks if Question belongs to an Assignment of which the group created the Exhibitor in
   * the app (Extra Round)
   */
  isCreatedExhibitor(): boolean {
    return this._id == 127;
  }

  /**
   * Getter for Id
   */
  get id(): number {
    return this._id;
  }

  /**
   * Getter for question
   */
  get questionText(): string {
    return this._questionText;
  }

  /**
   * Getter for answer
   */
  get answer(): string {
    return this._answer;
  }
  /**
   * Getter for answer
   */
  get category() {
    return this._category;
  }
  /**
   * Setter for category
   */
  set category(cat) {
    this._category = cat;
  }
  /**
   * Getter for exhibitor
   */
  get exhibitor() {
    return this._exhibitor;
  }
  /**
   * Setter for exhibitor
   */
  set exhibitor(exhibitor) {
    this._exhibitor = exhibitor;
  }
}
