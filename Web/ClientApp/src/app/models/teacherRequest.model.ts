export class TeacherRequest {

  /**
   * @ignore
   */
  private _name: string;
  private _surname: string;
  private _email: string;
  private _schoolName: string;
  private _note: string;


  /**
   * Constructor
   *
   * @param name
   * @param surname
   * @param email
   * @param schoolName
   * @param note
   */
  constructor(
    name: string,
    surname: string,
    email: string,
    schoolName: string,
    note: string,
  ) {
    this._name = name;
    this._surname = surname;
    this._email = email;
    this._schoolName = schoolName;
    this._note = note;
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
      json.note
    );
    return rec;
  }

  /**
   * Object to JSON parser
   */
  toJSON() {
    return {
      name: this._name,
      surname: this._surname,
      email: this._email,
      schoolName: this._schoolName,
      note: this._note,
    };
  }

  /**
   * Getters
   */
  get name(): string {
    return this._name;
  }

  set name(name) {
    this._name = name;
  }

  /**
   * Getter
   */
  get surname(): string {
    return this._surname;
  }

  set surname(surname) {
    this._surname = surname;
  }

  /**
   * Getter
   */
  get email(): string {
    return this._email;
  }

  set email(email) {
    this._email = email;
  }

  /**
   * Getter
   */
  get schoolName(): string {
    return this._schoolName;
  }

  set schoolName(schoolName) {
    this._schoolName = schoolName;
  }

  /**
   * Getter
   */
  get note(): string {
    return this._note;
  }

  set note(note) {
    this._note = note;
  }
}
