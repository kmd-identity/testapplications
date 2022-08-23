import { Component, OnInit } from '@angular/core';
import { ConfigIds, IdentityProviders } from '../config/auth-config.module';
import { AuthenticationContext } from './authentication-context.service';

@Component({
  selector: 'app-authenticate',
  templateUrl: './authenticate.component.html',
  styleUrls: ['./authenticate.component.css']
})
export class AuthenticateComponent implements OnInit {

  domainHint = "";
  domainHints: string[] = [
    IdentityProviders.KmdAd, 
    IdentityProviders.ContextHandlerTestApplications, 
    IdentityProviders.NemloginThreeTestPublic, 
    IdentityProviders.NemloginThreeTestPrivate]
    
  constructor(private authenticationContext: AuthenticationContext) { }

  ngOnInit(): void {
  }

  login() {
    // todo: This will be removed when approaching completion of the work. For now, this is a developer convenience
    // todo: This MUST be removed before merging.
    // this.domainHint = this.domainHint === "" ? IdentityProviders.KmdAd : this.domainHint;

    this.authenticationContext.login(ConfigIds.Code, this.domainHint);
  }

}
