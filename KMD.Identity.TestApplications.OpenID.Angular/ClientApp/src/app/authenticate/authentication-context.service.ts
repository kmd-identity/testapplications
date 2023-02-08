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

  public accessToken(): string {
    return this.oidcSecurityService.getAccessToken(ConfigIds.Code);
  }

  public idToken(): string {
    return this.oidcSecurityService.getIdToken(ConfigIds.Code);
  }
  
  private codeLogin = new BehaviorSubject<LoginResponse | undefined>(undefined);
  public codeLogin$ = this.codeLogin.asObservable();

  private tokenExchangeLogin = new BehaviorSubject<LoginResponse | undefined>(undefined);
  public tokenExchangeLogin$ = this.tokenExchangeLogin.asObservable();

  constructor(
    private oidcSecurityService: OidcSecurityService, 
    private errorService: ErrorService,
    private appConfig: AppConfig) { 

      this.oidcSecurityService.checkAuth().subscribe((loginResponse: LoginResponse) => {

        if(loginResponse.configId === ConfigIds.Code) {
          this.codeLogin.next(loginResponse);
        } else if (loginResponse.configId === ConfigIds.UserDelegation) {
          this.tokenExchangeLogin.next(loginResponse);
        }
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
        });
    }

  login(configId: string, domainHint: string | undefined) {
    this.errorService.reset();
    this.oidcSecurityService.authorize(configId, { customParams: { "domain_hint": domainHint ?? "" } });

    //To use the Unilogin connection with a different flow than the default (one factor),
    //add a query string parameter, to read more about this go to our Wiki, example below: 
    //this.oidcSecurityService.authorize(configId, { customParams: { "domain_hint": domainHint ?? "", "unilogin_loa": "TwoFactor" } });
  }

  logout() {
    const authOptions = {
      customParams: {
        client_id: this.appConfig.security.clientId
      }
    };
    
    this.errorService.reset();
    this.oidcSecurityService.logoff(undefined, authOptions);
    this.oidcSecurityService.logoffLocalMultiple();
    this.codeLogin.next(undefined);
    this.tokenExchangeLogin.next(undefined);
  }

  userData(): any {
    return this.oidcSecurityService.getUserData();
  }
}
