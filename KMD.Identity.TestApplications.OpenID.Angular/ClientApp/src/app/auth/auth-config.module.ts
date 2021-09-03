import { NgModule } from '@angular/core';
import { AuthModule, StsConfigLoader, StsConfigHttpLoader } from 'angular-auth-oidc-client';
import { AppConfig } from '../config/app.config';

export const httpLoaderFactory = (appConfig: AppConfig) => {
  
  const origin: string = window.location.origin;

  const config$ = appConfig.ensureLoaded()
    .then(() => Promise.resolve(
      {
        configId: "identitykmddk",
        authority: appConfig.get('authority'),
        redirectUrl: origin,
        postLogoutRedirectUri: origin,
        clientId: appConfig.get('clientId'),
        scope: appConfig.get('apiScope'),
        responseType: 'code',
        autoUserInfo: false,
        silentRenew: true,
        useRefreshToken: true,
        renewTimeBeforeTokenExpiresInSeconds: 30,
      }));

  return new StsConfigHttpLoader(config$);
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
