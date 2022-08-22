import { Injectable } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';
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
    private  oidcSecurityService: OidcSecurityService) { 
  }

  public requestDelegationToken() {
    // maybe this isn't needed to be observable at all.
    let userData = this.oidcSecurityService.getUserData(ConfigIds.Code)
    this.delegationToken.next(userData);
  }
}
