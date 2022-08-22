import { Injectable } from '@angular/core';
import { OidcSecurityService, LoginResponse } from 'angular-auth-oidc-client';
import { BehaviorSubject } from 'rxjs';
import { AppConfig } from '../config/app.config';
import { ConfigIds } from '../config/auth-config.module';
import { ErrorService } from '../error.service';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationContext {

  private _accessToken: string = '';
  public accessToken(): string {
    return this._accessToken;
  }

  private _idToken: string = '';
  public idToken(): string {
    return this._idToken;
  }

  private isAuthenticated = new BehaviorSubject<boolean>(false)
  public isAuthenticated$ = this.isAuthenticated.asObservable();

  constructor(
    private oidcSecurityService: OidcSecurityService, 
    private errorService: ErrorService,
    private appConfig: AppConfig) { 

      this.oidcSecurityService.checkAuth()
        .subscribe((loginResponse: LoginResponse) => {
          this.isAuthenticated.next(loginResponse.isAuthenticated);
          this._accessToken = loginResponse.accessToken;
          this._idToken = loginResponse.idToken;


          if (!loginResponse.isAuthenticated) {
            const urlParams = new URLSearchParams(window.location.search);
            const oidcError = urlParams.get('error');
            const oidcErrorDescription = urlParams.get('error_description');

            if (oidcError || oidcErrorDescription) {
              const error = {
                'Error': oidcError,
                'Description': oidcErrorDescription
              };

              this.errorService.raise(error);
            }
          }
        })
    }

  login(domainHint: string | null) {
    this.errorService.reset();
    this.oidcSecurityService.authorize(ConfigIds.Code, { customParams: { "domain_hint": domainHint ?? "" } });
  }

  logout() {
    const authOptions = {
      customParams: {
        "client_id": this.appConfig.security.clientId,
      }
    };
    
    this.errorService.reset();
    this.oidcSecurityService.logoff(ConfigIds.Code, authOptions);
    this.isAuthenticated.next(false);
  }

  userData(): any {
    return this.oidcSecurityService.getUserData();
  }
}