import {Injectable} from "@angular/core";
import "rxjs-compat/add/observable/interval";

@Injectable()
export class AppShareService {
  private _alerts: any[] = [];

  get alerts() {
    return this._alerts;
  }

  addAlert(alert: any) {
    this._alerts.push(alert);
  }
}
