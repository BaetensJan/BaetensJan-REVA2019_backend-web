import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {map} from "rxjs/operators";

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

  getApplicationStartDate(): Observable<any> {
    return this.http
      .get(`${this._appUrl}/GetStartDateOfApplication/`)
      .pipe(map((d: any) => d.startDate))
  }
}
