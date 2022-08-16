import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AuthConfigModule } from './auth/auth-config.module';
import { FormsModule } from '@angular/forms'
import { HttpClientModule } from '@angular/common/http';
import { AppConfig } from './config/app.config';
import { UserDelegationComponent } from './user-delegation/user-delegation.component';
import { ViewApiResponseComponent } from './view-api-response/view-api-response.component';
import { ErrorComponent } from './error/error.component';
import { UserClaimsComponent } from './user-claims/user-claims.component';

@NgModule({
  declarations: [
    AppComponent,
    UserDelegationComponent,
    ViewApiResponseComponent,
    ErrorComponent,
    UserClaimsComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
    AuthConfigModule
  ],
  providers: [
    AppConfig
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
