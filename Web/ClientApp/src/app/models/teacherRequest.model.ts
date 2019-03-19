export class TeacherRequest {
  /**
   * Constructor
   *
   * @param name
   * @param surname
   * @param email
   * @param schoolName
   * @param note
   * @param accepted
   */
  constructor(
    name: string,
    surname: string,
    email: string,
    schoolName: string,
    note: string,
    accepted?: boolean) {
    this._name = name;
    this._surname = surname;
    this._email = email;
    this._schoolName = schoolName;
    this._note = note;
    this._accepted = accepted;
  }

  /**
   * @ignore
   */
  private _id: number;

  /**
   * Getter for Id
   */
  get id(): number {
    return this._id;
  }

  set id(id) {
    this._id = id;
  }

  private _name: string;

  /**
   * Getters
   */
  get name(): string {
    return this._name;
  }

  set name(name) {
    this._name = name;
  }

  private _surname: string;

  /**
   * Getter
   */
  get surname(): string {
    return this._surname;
  }

  set surname(surname) {
    this._surname = surname;
  }

  private _email: string;

  /**
   * Getter
   */
  get email(): string {
    return this._email;
  }

  set email(email) {
    this._email = email;
  }

  private _schoolName: string;

  /**
   * Getter
   */
  get schoolName(): string {
    return this._schoolName;
  }

  set schoolName(schoolName) {
    this._schoolName = schoolName;
  }

  private _note: string;

  /**
   * Getter
   */
  get note(): string {
    return this._note;
  }

  set note(note) {
    this._note = note;
  }

  private _creationDate: Date;

  /**
   * Getter
   */
  get creationDate(): Date {
    return this._creationDate;
  }

  set creationDate(creationDate) {
    this._creationDate = creationDate;
  }

  get creationDateString(): string {
    return this._creationDate.toLocaleDateString()
  }

  private _accepted?: boolean;
  get accepted() {
    return this._accepted;
  }

  /**
   * Static JSON to Object parser
   *
   * @param json
   */
  static fromJSON(json: any): TeacherRequest {
    const rec = new TeacherRequest(
      json.name,
      json.surname,
      json.email,
      json.schoolName,
      json.note,
      json.accepted,
    );
    rec.creationDate = new Date(json.creationDate);
    rec._id = json.id;
    return rec;
  }

  /**
   * Object to JSON parser
   */
  toJSON() {
    return {
      id: this._id,
      name: this._name,
      surname: this._surname,
      email: this._email,
      schoolName: this._schoolName,
      note: this._note,
      creationDate: this._creationDate,
      accepted: this._accepted,
    };
  }
}
