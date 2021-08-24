import { NgModule } from '@angular/core';
import { AuthModule } from 'angular-auth-oidc-client';


@NgModule({
  imports: [AuthModule.forRoot({
    config: {
      configId: "identitykmddk",
      authority: 'https://identity.kmd.dk/adfs',
      redirectUrl: window.location.origin,
      postLogoutRedirectUri: window.location.origin,
      clientId: 'c65e6230-22ac-4aaa-8c2b-ef421fee8cbe',
      scope: 'urn:kmd-identity-test-application-api.local/user_impersonation openid allatclaims',
      responseType: 'code',
      autoUserInfo: false,
      silentRenew: true,
      useRefreshToken: true,
      renewTimeBeforeTokenExpiresInSeconds: 30,
    }
  })],
  exports: [AuthModule],
})
export class AuthConfigModule { }
