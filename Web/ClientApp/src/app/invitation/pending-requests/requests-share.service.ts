import {Injectable} from "@angular/core";
import {TeacherRequest} from "../../models/teacherRequest.model";

@Injectable({
  providedIn: 'root'
})
export class RequestsShareService {
  /**
   * Is it an edit or a create
   */
  private _aanpassen: boolean;
  /**
   * @ignore
   */
  private _teacherRequest: TeacherRequest;

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
   * Getter for TeacherRequest
   */
  get teacherRequest(): TeacherRequest {
    return this._teacherRequest;
  }

  /**
   * Setter for TeacherRequest
   *
   * @param value
   */
  set teacherRequest(value: TeacherRequest) {
    this._teacherRequest = value;
  }
}
