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
  private readonly _name: string;

  private _schoolName: string;
  /**
   * @ignore
   */
  private readonly _assignments: Assignment[];
  /**
   * @ignore
   */
  private readonly _members: string[];

  /**
   * Constructor
   *
   * @param name
   * @param assignments
   * @param members
   */
  constructor(name: string, assignments: Assignment[], members: string[]) {
    this._name = name;
    if (assignments) assignments.forEach(a => a.question = Question.fromJSON(a.question));

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
  static fromJSON(json: any): Group {
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
  get id(): number {
    return this._id;
  }

  /**
   * Getter for name
   */
  get name(): string {
    return this._name;
  }

  /**
   * Getter for schoolAndGroupName
   * used in groups and assignments overview so that an admin also sees the schoolName
   */
  public get schoolAndGroupName() {
    return `${this._schoolName} ${this._name}`;
  }

  /**
   * Setter for schoolName
   * used in groups and assignments overview so that an admin also sees the schoolName
   */
  set schoolName(name: string) {
    this._schoolName = name;
  }

  get schoolName(): string {
    return this._schoolName;
  }

  /**
   * Getter for FinishedAssignments
   */
  get assignments(): Assignment[] {
    return this._assignments;
  }

  get members(): string[] {
    return this._members;
  }
}
