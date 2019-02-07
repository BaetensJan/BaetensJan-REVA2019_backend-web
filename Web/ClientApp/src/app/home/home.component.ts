import {Component} from '@angular/core';
import {Observable, Subscription} from "rxjs";
import {ExhibitorsDataService} from "../exhibitors/exhibitors-data.service";
import {ExhibitorShareService} from "../exhibitor/exhibitor-share.service";
import {Exhibitor} from "../models/exhibitor.model";
import {forEach} from "@angular/router/src/utils/collection";

/**
 * @ignore
 */
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {

}
