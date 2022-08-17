import { HttpClient, HttpHeaders } from '@angular/common/http';
import { OnInit, Component } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';
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

  ngOnInit() {
    this.oidcSecurityService.checkAuth().subscribe(({ isAuthenticated, userData, accessToken, idToken }) => {
      this.isAuthenticated = isAuthenticated;
      this.userData = userData;
      this.accessToken = accessToken;
      this.idToken = idToken;

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
    this.oidcSecurityService.authorize("identitykmddk", { customParams: { "domain_hint": this.domainHint } });
  }

  logout() {
    const authOptions = {
      customParams: {
        "client_id": this.appConfig.get("clientId"),
      }
    };
    this.error = null;
    this.oidcSecurityService.logoff("identitykmddk", authOptions);
  }
  
  callApi() {
    this.showTestApiCall = true;
    this.testApiCallService.callTestApi(this.accessToken);
  }
}

