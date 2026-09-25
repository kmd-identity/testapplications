import { Component, Input, OnInit } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { map } from 'rxjs/operators';
import { ConfigIds } from '../config/auth-config.module';

@Component({
  selector: 'app-user-claims',
  templateUrl: './user-claims.component.html',
  styleUrls: ['./user-claims.component.css'],
  standalone: false
})
export class UserClaimsComponent implements OnInit {

  public userClaims: any;

  constructor(private oidcSecurityService: OidcSecurityService) { }

  ngOnInit(): void {
    this.userClaims = this.oidcSecurityService.getUserData(ConfigIds.Code).pipe(
      map(userData => this.sortClaims(userData))
    );
  }

  private sortClaims(userData: any): any {
    if (!userData || Array.isArray(userData) || typeof userData !== 'object') {
      return userData;
    }

    return Object.keys(userData)
      .sort((left, right) => left.toLowerCase().localeCompare(right.toLowerCase()))
      .reduce((sortedUserData: any, claimType) => {
        Object.defineProperty(sortedUserData, claimType, {
          value: userData[claimType],
          enumerable: true,
          configurable: true,
          writable: true
        });
        return sortedUserData;
      }, {});
  }
}
