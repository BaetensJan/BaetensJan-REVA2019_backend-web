import {HTTP_INTERCEPTORS} from '@angular/common/http';
import {AuthenticationInterceptor} from './authentication-interceptor';

/**
 * @ignore
 */
export const httpInterceptorProviders = [
  {
    provide: HTTP_INTERCEPTORS,
    useClass: AuthenticationInterceptor,
    multi: true
  }
];
