import { Injectable } from '@angular/core';
import { OidcSecurityService, AuthOptions } from 'angular-auth-oidc-client';
import { BehaviorSubject } from 'rxjs';
import { AuthenticationContext } from '../authenticate/authentication-context.service';
import { ConfigIds, IdentityProviders } from '../config/auth-config.module';

@Injectable({
  providedIn: 'root'
})
export class UserDelegationService {

  private delegationToken = new BehaviorSubject<any>(null);
  public delgationToken$ = this.delegationToken.asObservable();

  constructor(
    private authenticationContext: AuthenticationContext,
    private oidcSecurityService: OidcSecurityService) {

      this.authenticationContext.tokenExchangeLogin$.subscribe(loginResponse => {
        const token = this.oidcSecurityService.getUserData(ConfigIds.UserDelegation);
        this.delegationToken.next(token);
      })
  }

  public requestTokenExchange() {

    const authOption: AuthOptions = {
      customParams: {
      "userdelegation_actas_nameid": "myself-for-now-until-implementation-is-provided",
      "userdelegation_actas_identityprovider": IdentityProviders.KmdAd,
      "domain_hint": IdentityProviders.KmdAd,
    }};
    
    this.oidcSecurityService.authorize(ConfigIds.UserDelegation, authOption);
  }
}
