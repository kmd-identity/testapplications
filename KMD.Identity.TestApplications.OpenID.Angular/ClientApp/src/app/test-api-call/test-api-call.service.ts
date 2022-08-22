import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AppConfig } from '../config/app.config';
import { AuthenticationContext } from '../authenticate/authentication-context.service';

@Injectable({
  providedIn: 'root'
})
export class TestApiCallService {

  private testApiResponse = new BehaviorSubject<any>(null);
  public testApiResponse$ = this.testApiResponse.asObservable();

  constructor(
    private authenticationContext: AuthenticationContext,
    private httpClient: HttpClient, 
    private appConfig: AppConfig) { }

  public callTestApi() {

    const bearerHeaders = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': 'Bearer ' + this.authenticationContext.accessToken()
    });

    this.httpClient.get(this.appConfig.security.apiUrl, { headers: bearerHeaders }).subscribe(response => this.testApiResponse.next(response));
  }
}
