import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AppConfig } from '../config/app.config';

@Injectable({
  providedIn: 'root'
})
export class TestApiCallService {

  private testApiResponse = new BehaviorSubject<any>(null);
  public testApiResponse$ = this.testApiResponse.asObservable();

  constructor(private httpClient: HttpClient, private appConfig: AppConfig) { }

  public callTestApi(accessToken: string) {

    const bearerHeaders = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': 'Bearer ' + accessToken
    });

    this.httpClient.get(this.appConfig.security.apiUrl, { headers: bearerHeaders }).subscribe(response => this.testApiResponse.next(response));
  }
}
