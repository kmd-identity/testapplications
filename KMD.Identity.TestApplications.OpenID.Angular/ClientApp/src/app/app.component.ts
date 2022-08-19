import { OnInit, Component } from '@angular/core';
import { OidcSecurityService, LoginResponse } from 'angular-auth-oidc-client';
import { ConfigIds, IdentityProviders } from './auth/auth-config.module';
import { AppConfig } from './config/app.config';
import { TestApiCallService } from './test-api-call/test-api-call.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  constructor(
    private oidcSecurityService: OidcSecurityService, 
    private appConfig: AppConfig,
    private testApiCallService: TestApiCallService) { }
  title = 'IdentityApp';
  isAuthenticated = false;
  userData: any = null;
  accessToken = "";
  idToken = "";
  domainHint = "";
  apiResponse: any = null;
  error: any = null;
  showTestApiCall: boolean = false;
  performUserDelegation: boolean = false;
  domainHints: string[] = [
    IdentityProviders.KmdAd, 
    IdentityProviders.ContextHandlerTestApplications, 
    IdentityProviders.NemloginThreeTestPublic, 
    IdentityProviders.NemloginThreeTestPrivate]

  ngOnInit() {
    this.oidcSecurityService.checkAuth().subscribe((loginResponse: LoginResponse) => {
      this.isAuthenticated = loginResponse.isAuthenticated;
      this.userData = loginResponse.userData;
      this.accessToken = loginResponse.accessToken;
      this.idToken = loginResponse.idToken;

      if (!this.isAuthenticated) {
        const urlParams = new URLSearchParams(window.location.search);
        const oidcError = urlParams.get('error');
        const oidcErrorDescription = urlParams.get('error_description');

        if (oidcError || oidcErrorDescription) {
          this.error = {
            'Error': oidcError,
            'Description': oidcErrorDescription
          }
        }
      }
    });

    // A quick fix to initiate the log-in flow immediately from https://test.identity.kmd.dk/
    if (window.location.search.indexOf('autologin') > -1) {
      this.login();
    }
  }

  login() {
    this.error = null;

    // // todo: This will be removed when approaching completion of the work. For now, this is a developer convenience
    // // todo: This MUST be removed before merging.
    // this.domainHint = this.domainHint === "" ? IdentityProviders.KmdAd : this.domainHint;

    this.oidcSecurityService.authorize(ConfigIds.Code, { customParams: { "domain_hint": this.domainHint } });
  }

  logout() {
    const authOptions = {
      customParams: {
        "client_id": this.appConfig.security.clientId,
      }
    };
    this.error = null;
    this.oidcSecurityService.logoff(ConfigIds.Code, authOptions);
  }
  
  callApi() {
    this.showTestApiCall = true;
    this.testApiCallService.callTestApi(this.accessToken);
  }

  beginUserDelegation() {
    this.performUserDelegation = true;
  }

  userDelegationEnabled() {
    return this.isAuthenticated
      && this.appConfig.featureToggle.userDelegation
      && this.userData 
      && this.userData["identityprovider"] == IdentityProviders.KmdAd;
  }
}

