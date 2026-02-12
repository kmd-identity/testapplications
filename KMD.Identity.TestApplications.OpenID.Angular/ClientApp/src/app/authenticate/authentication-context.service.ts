import { Injectable } from '@angular/core';
import { OidcSecurityService, LoginResponse } from 'angular-auth-oidc-client';
import { BehaviorSubject, Observable } from 'rxjs';
import { AppConfig } from '../config/app.config';
import { ConfigIds } from '../config/auth-config.module';
import { ErrorService } from '../error.service';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationContext {

  public accessToken(): Observable<string> {
    return this.oidcSecurityService.getAccessToken(ConfigIds.Code);
  }

  public idToken(): Observable<string> {
    return this.oidcSecurityService.getIdToken(ConfigIds.Code);
  }
  
  private codeLogin = new BehaviorSubject<LoginResponse | undefined>(undefined);
  public codeLogin$ = this.codeLogin.asObservable();

  constructor(
    private oidcSecurityService: OidcSecurityService, 
    private errorService: ErrorService,
    private appConfig: AppConfig) { 

      console.log("Initializing authentication context");

      this.oidcSecurityService.checkAuth().subscribe((loginResponse: LoginResponse) => {

        console.log("Authentication check completed", loginResponse);

        if(loginResponse.configId === ConfigIds.Code) {
          console.log("User is authenticated with code flow, updating authentication context");
          this.codeLogin.next(loginResponse);
        } 

          if (!loginResponse.isAuthenticated) {
            const urlParams = new URLSearchParams(window.location.search);
            const oidcError = urlParams.get('error');
            const oidcErrorDescription = urlParams.get('error_description');

            console.log("User is not authenticated, checking for OIDC errors in query parameters", { oidcError, oidcErrorDescription });

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

  login(configId: string, domainHint?: string, loginHint?: string) {
    this.errorService.reset();
    this.oidcSecurityService.authorize(configId, { customParams: { "domain_hint": domainHint ?? "", "login_hint": loginHint ?? "" } });

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
    this.oidcSecurityService.logoff(undefined, authOptions).subscribe(() => {
      this.codeLogin.next(undefined);
    });
    this.oidcSecurityService.logoffLocalMultiple();
  }

  userData(): any {
    return this.oidcSecurityService.getUserData();
  }
}
