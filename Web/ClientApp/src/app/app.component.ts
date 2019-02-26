import {Component} from '@angular/core';
import {AlertComponent} from "ngx-bootstrap";
import {AppShareService} from "./app-share.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  /**
   * App title
   */
  title = 'app';

  alerts: any[]; // alerts that appear above the page (e.g. when a new group is successfully created).

  constructor(private appShareService: AppShareService) {
    this.alerts = this.appShareService.alerts;
  }

  onClosed(dismissedAlert: AlertComponent): void {
    this.alerts.filter(alert => alert !== dismissedAlert);
  }
}
