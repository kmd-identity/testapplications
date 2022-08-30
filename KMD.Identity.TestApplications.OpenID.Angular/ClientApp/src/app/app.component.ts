import { OnInit, Component } from '@angular/core';
import { ConfigIds, IdentityProviders } from './config/auth-config.module';
import { AppConfig } from './config/app.config';
import { AuthenticationContext } from './authenticate/authentication-context.service';
import { ErrorService } from './error.service';
import { LoginResponse } from 'angular-auth-oidc-client';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  constructor(
    private authenticationContext: AuthenticationContext,
    private errorService: ErrorService,
    private appConfig: AppConfig) { }

  title = 'IdentityApp';
  isAuthenticated = false;
  hasDelegated = false;
  userData: any = null;
  isError: boolean = false;
  showTestApiCall: boolean = false;
  showUserDelegation: boolean = false;
  hackForceHideUserDelegation: boolean = false;
  codeLogin: LoginResponse | undefined = undefined;
  tokenExchangeLogin: LoginResponse | undefined = undefined;

  ngOnInit() {
    this.errorService.isError$.subscribe(isError => this.isError = isError);
    this.authenticationContext.codeLogin$.subscribe(
        loginResponse => {
          this.codeLogin = loginResponse;
      });

      this.authenticationContext.tokenExchangeLogin$.subscribe(
        loginResponse => {
          this.tokenExchangeLogin = loginResponse;
        }
      )
    this.InitiateAutoLogInFlowIfConfigured();
  }

  requireAuthentication(): boolean {
    return !(this.codeLogin?.isAuthenticated)
    && (!this.tokenExchangeLogin?.isAuthenticated)
  }

  InitiateAutoLogInFlowIfConfigured() {
    if (window.location.search.indexOf('autologin') > -1) {
      this.login();
    }
  }

  login() {
    this.showTestApiCall = false;
    this.showUserDelegation = false;

    this.authenticationContext.login(ConfigIds.Code, undefined);
  }

  logout() {
    this.showTestApiCall = false;
    this.showUserDelegation = false;

    this.authenticationContext.logout()
  }
  
  callApi() {
    this.showTestApiCall = true;
    this.showUserDelegation = false;
    this.hackForceHideUserDelegation = true;
  }

  beginUserDelegation() {

    this.showTestApiCall = false;
    this.showUserDelegation = true;
    this.hackForceHideUserDelegation = false;
  }

  public performUserDelegation(): boolean {
    return !this.hackForceHideUserDelegation // todo: clean up this abomination
    && (this.showUserDelegation 
    || (this.userDelegationEnabled() && this.tokenExchangeLogin?.isAuthenticated))
  }

  userDelegationEnabled() {

    return this.appConfig.featureToggle.userDelegation
      && this.authenticationContext.userData()
      && this.authenticationContext.userData()["identityprovider"] == IdentityProviders.KmdAd
      ;
  }
}

