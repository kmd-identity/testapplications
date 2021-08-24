import { HttpClient, HttpHeaders } from '@angular/common/http';
import { OnInit } from '@angular/core';
import { Component } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  constructor(public oidcSecurityService: OidcSecurityService, public http: HttpClient) { }
  title = 'IdentityApp';
  isAuthenticated = false;
  userData = null;
  accessToken = "";
  idToken = "";
  domainHint = "";
  apiResponse: any = null;

  ngOnInit() {
    this.oidcSecurityService.checkAuth().subscribe(({ isAuthenticated, userData, accessToken, idToken }) => {
      this.isAuthenticated = isAuthenticated;
      this.userData = userData;
      this.accessToken = accessToken;
      this.idToken = idToken
    });
  }

  login() {
    this.oidcSecurityService.authorize("identitykmddk", { customParams: { "domain_hint": this.domainHint } });
  }

  logout() {
    this.oidcSecurityService.logoff("identitykmddk");
  }

  async callApi() {

    const bearerHeaders = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': 'Bearer ' + this.accessToken
    })

    this.apiResponse = await this.http.get("https://localhost:44377/api/claims", { headers: bearerHeaders }).toPromise();
  }
}

