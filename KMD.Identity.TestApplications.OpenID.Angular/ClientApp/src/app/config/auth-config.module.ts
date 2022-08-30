import { NgModule } from '@angular/core';
import { AuthModule, StsConfigLoader, StsConfigHttpLoader, OpenIdConfiguration } from 'angular-auth-oidc-client';
import { AppConfig } from './app.config';

export const ConfigIds = {
  Code: "identitykmddk",
  UserDelegation: "identitykmddkuserdelegation"
};

export const IdentityProviders = {
  KmdAd: "kmd-ad-prod",
  ContextHandlerTestApplications: "contexthandler-test-kmdidentitytestapplications",
  NemloginThreeTestPublic: "nemlogin-3-test-public",
  NemloginThreeTestPrivate: "nemlogin-3-test-private"
}

export const httpLoaderFactory = (appConfig: AppConfig) => {
  
  const origin: string = window.location.origin;

  const codeResponseTypeConfig$ = appConfig.ensureLoaded()
    .then(() => { 
      
      const codeResponseType: OpenIdConfiguration = {
        configId: ConfigIds.Code,
        authority: appConfig.security.authority,
        redirectUrl: origin,
        postLogoutRedirectUri: origin,
        clientId: appConfig.security.clientId,
        scope: appConfig.security.apiScope,
        responseType: 'code',
        autoUserInfo: false,
        silentRenew: true,
        useRefreshToken: true,
        renewTimeBeforeTokenExpiresInSeconds: 30
      }

      return Promise.resolve(codeResponseType);
    });

    // While this profile is identical to the one above, it allows us to reinitiate an authentication flow in the code that provides a user delegation token.
    // For now I'd like to maintain this as a second user delegation OpenIdConfiguration, as it makes it easier to juggle the standard- and user delegation- tokens
      
  const tokenExchangeTypeConfig$ = appConfig.ensureLoaded()
  .then(() => {
    const tokenExchangeResponseType : OpenIdConfiguration = {
      configId: ConfigIds.UserDelegation,
      authority: appConfig.security.authority,
      redirectUrl: origin,
      postLogoutRedirectUri: origin,
      clientId: appConfig.security.clientId,
      scope: appConfig.security.apiScope,
      responseType: 'code',
      autoUserInfo: false,
      silentRenew: true,
      useRefreshToken: true,
      renewTimeBeforeTokenExpiresInSeconds: 30
    }

    return Promise.resolve(tokenExchangeResponseType);
  });

  return new StsConfigHttpLoader([codeResponseTypeConfig$, tokenExchangeTypeConfig$]);
};

@NgModule({
  imports: [
    AuthModule.forRoot({
      loader: {
        provide: StsConfigLoader,
        useFactory: httpLoaderFactory,
        deps: [AppConfig],
      },
    })
  ],
  exports: [AuthModule]
})
export class AuthConfigModule {}
