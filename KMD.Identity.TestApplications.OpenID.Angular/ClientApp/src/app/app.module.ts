import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AuthConfigModule } from './config/auth-config.module';
import { FormsModule } from '@angular/forms'
import { HttpClientModule } from '@angular/common/http';
import { AppConfig } from './config/app.config';
import { UserDelegationComponent } from './user-delegation/user-delegation.component';
import { ErrorComponent } from './error/error.component';
import { UserClaimsComponent } from './user-claims/user-claims.component';
import { TestApiCallComponent } from './test-api-call/test-api-call.component';
import { AuthenticateComponent } from './authenticate/authenticate.component';

@NgModule({
  declarations: [
    AppComponent,
    UserDelegationComponent,
    ErrorComponent,
    UserClaimsComponent,
    TestApiCallComponent,
    AuthenticateComponent,
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
