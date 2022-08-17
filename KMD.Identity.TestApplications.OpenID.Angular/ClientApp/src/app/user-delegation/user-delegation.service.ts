import { Injectable } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { BehaviorSubject } from 'rxjs';
import { ConfigIds } from '../auth/auth-config.module';

@Injectable({
  providedIn: 'root'
})
export class UserDelegationService {

  private delegationToken = new BehaviorSubject<any>(null);
  public delgationToken$ = this.delegationToken.asObservable();

  constructor(private  oidcSecurityService: OidcSecurityService) { 
  }

  public requestDelegationToken() {
    // maybe this isn't needed to be observable at all.
    // const token = this.oidcSecurityService.getIdToken(ConfigIds.Code);
    let userData = this.oidcSecurityService.getUserData(ConfigIds.Code)
    // const token = this.oidcSecurityService.getAccessToken(ConfigIds.Code);
    this.delegationToken.next(userData);
  }
}
