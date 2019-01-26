import {Assignment} from "./assignment.model";
import {Question} from "./question.model";

export class Group {
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
  private _assignments: Assignment[];
  /**
   * @ignore
   */
  private _members: String[];

  /**
   * Constructor
   *
   * @param name
   * @param assignments
   * @param members
   */
  constructor(name: string, assignments: Assignment[], members: String[]) {
    this._name = name;
    if(assignments) assignments.forEach(a => a.question = Question.fromJSON(a.question));

    this._assignments = assignments;
    if (!members) {
      this._members = [];
    } else this._members = members;
  }

  /**
   * Static JSON to Object parser
   *
   * @param json
   */
  static

  fromJSON(json
             :
             any
  ):
    Group {
    const rec = new Group(json.name, json.assignments, json.members);

    rec._id = json.id;
    return rec;
  }

  /**
   * Object to JSON converter
   */
  toJSON() {
    return {
      id: this._id,
      name: this._name,
      assignments: this._assignments,
      members: this._members
    };
  }

  /**
   * Getter for Id
   */
  get id()
    :
    number {
    return this._id;
  }

  /**
   * Getter for name
   */
  get name()
    :
    string {
    return this._name;
  }

  /**
   * Getter for FinishedAssignments
   */
  get assignments()
    :
    Assignment[] {
    return this._assignments;
  }

  get members()
    :
    String[] {
    return this._members;
  }
}
