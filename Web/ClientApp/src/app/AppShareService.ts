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

  // hier met 1 modal werken doorheen de hele webapp, idpv voor elke component opnieuw aan te maken.
  // En de strings hier ook bewaren (reusability)
//  this.modalMessage = `Ben je zeker dat de groep met groepsnaam ${group.name} verwijderd mag worden? De ingediende opdrachten van deze groep worden hierdoor ook verwijderd.`;
}
