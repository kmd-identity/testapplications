import { HttpClient, HttpHeaders } from '@angular/common/http';
import { unescapeIdentifier } from '@angular/compiler';
import { OnInit, Component } from '@angular/core';
import { OidcSecurityService, LoginResponse } from 'angular-auth-oidc-client';
import { ConfigIds } from './auth/auth-config.module';
import { AppConfig } from './config/app.config';
import { TestApiCallService } from './test-api-call/test-api-call.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  constructor(public oidcSecurityService: OidcSecurityService, public http: HttpClient, private appConfig: AppConfig, private testApiCallService: TestApiCallService) { }
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

  ngOnInit() {
    this.oidcSecurityService.checkAuth().subscribe((loginResponse: LoginResponse) => {
        // todo: Limit this to only the ConfigIds.Code
    // this.oidcSecurityService.checkAuthMultiple(ConfigIds.Code).subscribe((loginResponses: LoginResponse[]) => {
      // const loginResponse = loginResponses.find(x => x.configId === ConfigIds.Code);
      // if(loginResponse === undefined) {return;}
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

    this.domainHint = this.domainHint === "" ? "kmd-ad-prod" : this.domainHint; //todo: This must be removed before merging. It's here as a convenience while I develop.

    this.oidcSecurityService.authorize(ConfigIds.Code, { customParams: { "domain_hint": this.domainHint } });
  }

  logout() {
    const authOptions = {
      customParams: {
        "client_id": this.appConfig.get("clientId"),
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
}

