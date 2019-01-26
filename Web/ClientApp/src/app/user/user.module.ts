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

/**
 * Routing for user login and registration
 */
const routes = [
  {path: 'login', component: LoginComponent},
  {path: 'logout', component: LogoutComponent},
  {path: 'register', component: RegisterComponent}
];

@NgModule({
  imports: [
    CommonModule,
    HttpModule,
    ReactiveFormsModule,
    RouterModule.forChild(routes)
  ],
  declarations: [LoginComponent, LogoutComponent, RegisterComponent],
  providers: [
    httpInterceptorProviders,
    AuthenticationService,
    AuthGuardService
  ],
  exports: []
})
export class UserModule {
}
