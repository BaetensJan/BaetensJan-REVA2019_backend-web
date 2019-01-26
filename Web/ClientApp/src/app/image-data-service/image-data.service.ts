import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs/Observable';
import {map} from "rxjs/operators";

@Injectable({
  providedIn: 'root'
})

export class ImageDataService {
  /**
   * Base url of the to connected connection
   */
  private readonly _appUrl = '/API/Image';
  private readonly _assetUrl = '/images/beursplan.jpg';

  /**
   * Constructor
   *
   * @param http
   */
  constructor(private http: HttpClient) {
  }

  /**
   * Makes call to the backend to upload a new routePlan image (update).
   *
   * Returns true if the server response was "beursplan.jpg" (succesfully updated the image)
   * @param image
   */
  UpdateRoutePlanImage(image: any): Observable<boolean> {
    console.log(typeof image);
    return this.http.post(`${this._appUrl}/UpdateExhibitionRoutePlanImage`, image).pipe(map((res: string) => {
      return res == "beursplan.jpg";
    }));
  }

  // GetRoutePlanImage(): Observable<any> {
  //   return this.http.get(`${this._appUrl}/GetExhibitionRoutePlanImage`).pipe(map((res: any) => {
  //     var file = new Blob([res.blob], {
  //       type: 'image/jpeg'
  //     });
  //     var fileURL = URL.createObjectURL(file);
  //     return fileURL;
  //   }));
  // }
}
