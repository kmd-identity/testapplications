import { NgModule } from '@angular/core';
import { AuthModule, StsConfigLoader, StsConfigHttpLoader, OpenIdConfiguration } from 'angular-auth-oidc-client';
import { AppConfig } from '../config/app.config';

export const ConfigIds = {
  Code: "identitykmddk",
  TokenExchange: "identitykmddkuserdelegation"
};

// this one compiles
export const httpLoaderFactory = (appConfig: AppConfig) => {
  
  const origin: string = window.location.origin;

  const codeResponseTypeConfig$ = appConfig.ensureLoaded()
    .then(() => { 
      
      const codeResponseType: OpenIdConfiguration = {
        configId: ConfigIds.Code,
        authority: appConfig.get('authority'),
        redirectUrl: origin,
        postLogoutRedirectUri: origin,
        clientId: appConfig.get('clientId'),
        scope: appConfig.get('apiScope'),
        responseType: 'code',
        autoUserInfo: false,
        silentRenew: true,
        useRefreshToken: true,
        renewTimeBeforeTokenExpiresInSeconds: 30
      }
      
      return Promise.resolve(codeResponseType)
      });
      
  const tokenExchangeTypeConfig$ = appConfig.ensureLoaded()
  .then(() => {
    const tokenExchangeResponseType : OpenIdConfiguration = {
      configId: ConfigIds.TokenExchange,
      authority: appConfig.get('authority'),
      redirectUrl: origin,
      postLogoutRedirectUri: origin,
      clientId: appConfig.get('clientId'),
      scope: appConfig.get('apiScope'),
      responseType: 'token-exchange',
      autoUserInfo: false,
      silentRenew: true,
      useRefreshToken: true,
      renewTimeBeforeTokenExpiresInSeconds: 30
    }
      
    return Promise.resolve(tokenExchangeResponseType)
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
