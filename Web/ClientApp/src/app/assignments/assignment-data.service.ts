import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs/index";
import {map} from "rxjs/operators/index";
import {Group} from "../models/group.model";
import {School} from "../models/school.model";

@Injectable({
  providedIn: 'root'
})
export class AssignmentDataService {
  /**
   * Base url of the to connected connection
   */
  private readonly _appUrl = '/API/Assignment';

  /**
   * Constructor
   *
   * @param http
   */
  constructor(private http: HttpClient) {
  }
}
