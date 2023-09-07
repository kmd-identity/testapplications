import { NgModule } from '@angular/core';
import { AuthModule, StsConfigLoader, StsConfigHttpLoader, OpenIdConfiguration } from 'angular-auth-oidc-client';
import { AppConfig } from './app.config';
import { from } from 'rxjs';

export const ConfigIds = {
  Code: "identitykmddk"
};

export const IdentityProviders = {
  KmdAd: "kmd-ad-prod",
  ContextHandlerTestApplications: "contexthandler-test-kmdidentitytestapplications",
  NemloginThreeTestPublic: "nemlogin-3-test-public",
  NemloginThreeTestPrivate: "nemlogin-3-test-private"
}

export const httpLoaderFactory = (appConfig: AppConfig) => {

  const origin: string = window.location.origin;

  const codeResponseTypeConfig$ = from(appConfig.ensureLoaded()
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
    }));

  return new StsConfigHttpLoader([codeResponseTypeConfig$]);
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
