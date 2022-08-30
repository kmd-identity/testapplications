import { Component, Input, OnInit } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { ConfigIds } from '../config/auth-config.module';

@Component({
  selector: 'app-user-claims',
  templateUrl: './user-claims.component.html',
  styleUrls: ['./user-claims.component.css']
})
export class UserClaimsComponent implements OnInit {

  public userClaims: any;

  constructor(private oidcSecurityService: OidcSecurityService) { }

  ngOnInit(): void {
    this.userClaims = this.oidcSecurityService.getUserData(ConfigIds.Code);
  }
}
