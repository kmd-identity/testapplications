import { OnInit, Component } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { IdentityProviders } from './config/auth-config.module';
import { AppConfig } from './config/app.config';
import { AuthenticationContext } from './authenticate/authentication-context.service';
import { ErrorService } from './error.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  constructor(
    private oidcSecurityService: OidcSecurityService, 
    private authenticationContext: AuthenticationContext,
    private errorService: ErrorService,
    private appConfig: AppConfig) { }

  title = 'IdentityApp';
  isAuthenticated = false;
  userData: any = null;
  isError: boolean = false;
  showTestApiCall: boolean = false;
  performUserDelegation: boolean = false;

  ngOnInit() {
    this.errorService.isError$.subscribe(isError => this.isError = isError);
    this.authenticationContext.isAuthenticated$.subscribe(isAuthenticated => 
      {
        this.userData = this.authenticationContext.userData();
        this.isAuthenticated = isAuthenticated;
      });

    // A quick fix to initiate the log-in flow immediately from https://test.identity.kmd.dk/
    if (window.location.search.indexOf('autologin') > -1) {
      this.login();
    }
  }

  login() {
    this.showTestApiCall = false;
    this.performUserDelegation = false;

    this.authenticationContext.login("");
  }

  logout() {
    this.showTestApiCall = false;
    this.performUserDelegation = false;

    this.authenticationContext.logout()
  }
  
  callApi() {
    this.showTestApiCall = true;
    this.performUserDelegation = false;
  }

  beginUserDelegation() {
    this.showTestApiCall = false;
    this.performUserDelegation = true;
  }

  userDelegationEnabled() {
    return this.isAuthenticated
      && this.appConfig.featureToggle.userDelegation
      && this.userData 
      && this.userData["identityprovider"] == IdentityProviders.KmdAd;
  }
}

