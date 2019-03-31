import {AfterViewInit, Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {ImageDataService} from "../image-data-service/image-data.service";
import 'rxjs/add/observable/timer';
import {AppShareService} from "../app-share.service";
import {Observable} from "rxjs";
import {AuthenticationService} from "../user/authentication.service";


@Component({
  selector: 'app-route-map',
  templateUrl: './route-map.component.html',
  styleUrls: ['./route-map.component.css']
})
export class RouteMapComponent implements AfterViewInit, OnInit {

  /**
   * @ignore
   */
  @ViewChild('myImage', {read: ElementRef}) public image: ElementRef;
  /**
   * @ignore
   */
  @ViewChild('myContainer', {read: ElementRef}) public container: ElementRef;
  public cx: CanvasRenderingContext2D;
  private _routePlanImage; //image url
  fileSelected = false;
  successMessage = "";
  showMessage = false;
  showMessageFail = false;

  /**
   * Constructor
   *
   * @param _imageDataService
   * @param _appShareService
   * @param _authService
   */
  constructor(
    private _imageDataService: ImageDataService,
    private _appShareService: AppShareService,
    private _authService: AuthenticationService,) {
  }

  ngOnInit() {
    this.loadImage();
  }

  get isAdmin(): Observable<boolean> {
    return this._authService.isModerator$;
  }

  /**
   * When user clicks on the 'upload' button after having selected an image in his folder directory.
   */
  uploadFile() {
    let formData = new FormData();

    formData.append('file', this.dataURItoBlob(this._routePlanImage));

    this._imageDataService.UpdateRoutePlanImage(formData).subscribe((value: boolean) => {
      if (value == false) {
        this._appShareService.addAlert({
          type: 'success',
          msg: `Het beursplan is succesvol geupload`,
          timeout: 5000
        });
      } else {
        this._appShareService.addAlert({
          type: 'success',
          msg: `Het uploaden van het beursplan is mislukt`,
          timeout: 5000
        });
      }
    });
  }

  /**
   * Convert base64 to raw binary data held in a string.
   * @param dataURI
   */
  private dataURItoBlob(dataURI) {
    // convert base64/URLEncoded data component to raw binary data held in a string
    let byteString;
    if (dataURI.split(',')[0].indexOf('base64') >= 0)
      byteString = atob(dataURI.split(',')[1]);
    else
      byteString = unescape(dataURI.split(',')[1]);

    // separate out the mime component
    let mimeString = dataURI.split(',')[0].split(':')[1].split(';')[0];

    // write the bytes of the string to a typed array
    let ia = new Uint8Array(byteString.length);
    for (let i = 0; i < byteString.length; i++) {
      ia[i] = byteString.charCodeAt(i);
    }

    return new Blob([ia], {type: mimeString});
  }

  ngAfterViewInit() {
    //x = scale element for the canvas (canvas resizes after screen resize & click event on canvas)
    let x = (<HTMLElement>this.container.nativeElement).clientWidth / this.image.nativeElement.width;
    this.image.nativeElement.width = this.image.nativeElement.width * x;
    this.image.nativeElement.height = this.image.nativeElement.height * x;
  }

  /**
   * Loads an URL to the RoutePlan, which will show the image in the HTML page
   */
  loadImage() {
    this._routePlanImage = `/images/beursplan.jpg?dummy=${new Date().getTime()}`;
  }

  onSelectFile(event) { // called each time file input changes
    let imageFile = event.target.files[0];
    if (event.target.files && imageFile) {
      let reader = new FileReader();

      reader.readAsDataURL(imageFile); // read file as data url

      reader.onload = (event: any) => { // called once readAsDataURL is completed
        this._routePlanImage = event.target.result;
        this.fileSelected = true;
      }
    }
  }

  set routePlanImage(file) {
    this._routePlanImage = file;
  }

  get routePlanImage() {
    return this._routePlanImage;
  }
}
