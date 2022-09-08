import { Injectable } from '@angular/core';
import { OidcSecurityService, AuthOptions } from 'angular-auth-oidc-client';
import { BehaviorSubject } from 'rxjs';
import { AuthenticationContext } from '../authenticate/authentication-context.service';
import { ConfigIds } from '../config/auth-config.module';

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

  public requestTokenExchange(myDomain: string, actAsSubjectDomain: string, consentGrantedMasquerade: boolean) {

    const authOption: AuthOptions = {
      customParams: {
      "userdelegation_actas_nameid": "myself-for-now-until-implementation-is-provided",
      "userdelegation_actas_identityprovider": actAsSubjectDomain,
      "domain_hint": myDomain,
      "userdelegation_endpoint_should_reply_with_stub_consent": consentGrantedMasquerade // keep in mind that this is a developer convenience while we deliver the feature. It WILL disappear
    }};
    
    this.oidcSecurityService.authorize(ConfigIds.UserDelegation, authOption);
  }
}
