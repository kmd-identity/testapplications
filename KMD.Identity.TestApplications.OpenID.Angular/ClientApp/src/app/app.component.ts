import { HttpClient, HttpHeaders } from '@angular/common/http';
import { OnInit, Component } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { AppConfig } from './config/app.config';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  constructor(public oidcSecurityService: OidcSecurityService, public http: HttpClient, private appConfig: AppConfig) { }
  title = 'IdentityApp';
  isAuthenticated = false;
  userData: any = null;
  accessToken = "";
  idToken = "";
  domainHint = "";
  apiResponse: any = null;
  error: any = null;

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
  }

  login() {
    this.error = null;
    this.oidcSecurityService.authorize("identitykmddk", { customParams: { "domain_hint": this.domainHint }});
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

  async callApi() {

    const bearerHeaders = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': 'Bearer ' + this.accessToken
    });

    this.apiResponse = await this.http.get(this.appConfig.get('apiUrl'), { headers: bearerHeaders }).toPromise();
  }
}

