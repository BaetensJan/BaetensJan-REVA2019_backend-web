import {ReactiveFormsModule} from '@angular/forms';
import {RouterModule} from '@angular/router';
import {LoginComponent} from './login/login.component';
import {AuthenticationService} from './authentication.service';
import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {HttpModule} from '@angular/http';
import {RegisterComponent} from './register/register.component';
import {httpInterceptorProviders} from '../http-interceptors';
import {AuthGuardService} from "./auth-guard.service";
import {LogoutComponent} from "./logout/logout.component";
import {InvitationService} from "./invitation/invitation.service";
import {RequestsComponent} from "./invitation/pending-requests/requests.component";
import {InviteRequestComponent} from "./invitation/send-request/invite-request.component";
import {ForgotPasswordComponent} from './forgot-password/forgot-password.component';
import { ForgotPasswordConfirmationComponent } from './forgot-password-confirmation/forgot-password-confirmation.component';

/**
 * Routing for user login and registration
 */
const routes = [
  {path: 'login', canActivate: [AuthGuardService], component: LoginComponent},
  {path: 'logout', canActivate: [AuthGuardService], component: LogoutComponent},
  {path: 'register', canActivate: [AuthGuardService], component: RegisterComponent},
  {path: 'wachtwoord-vergeten', canActivate: [AuthGuardService], component: ForgotPasswordComponent}
];

@NgModule({
  imports: [
    CommonModule,
    HttpModule,
    ReactiveFormsModule,
    RouterModule.forChild(routes)
  ],
  declarations: [LoginComponent, LogoutComponent, RegisterComponent, InviteRequestComponent, RequestsComponent, ForgotPasswordComponent, ForgotPasswordConfirmationComponent],
  providers: [
    httpInterceptorProviders,
    InvitationService,
    AuthenticationService,
    AuthGuardService
  ],
  exports: []
})
export class UserModule {
}
