import {Group} from "./group.model";

export class School {

    /**
     * @ignore
     */
    private _id: number;
    /**
     * @ignore
     */
    private _name: string;
    private _groups: Group[];
    private _password: string;
    private _start: Date;

    /**
     * Constructor
     *
     * @param name
     * @param password
     * @param groups
     * @param start
     */
    constructor(name: string, password: string, groups: Group[], start: Date = null) {
      this._name = name;
      this._password = password;
      this._groups = groups;
      this._start = start ? start : new Date();
    }

    /**
     * Static JSON to Object parser
     *
     * @param json
     */
    static fromJSON(json: any): School {
      const rec = new School(
        json.name,
        json.password,
        json.groups,
        json.start);

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
        groups: this._groups,
        password: this._password,
        start: this._start
      };
    }

    /**
     * Getter for Id
     */
    get id(): number {
      return this._id;
    }

    /**
     * Getter for password (login in android application)
     */
    get password(): string {
      return this._password;
    }

    /**
     * Getter for name
     */
    get name(): string {
      return this._name;
    }

    get groups(): Group[] {
      return this._groups;
    }

    get start(): Date {
      return this._start;
    }
  }
