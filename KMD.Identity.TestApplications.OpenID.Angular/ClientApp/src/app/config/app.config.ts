import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class AppConfig {

  private config: any = null;
  public security: SecurityConfig = null!;  
  public featureToggle: FeatureToggle = null!;

  constructor(private http: HttpClient) {
  }

  public async ensureLoaded() {
    if (this.config == null) {
      await this.load();
    }
    return Promise.resolve(true);
  }

  private async load() {
    this.config = await this.http.get('config/appconfig').toPromise();
    this.security = this.config.security;
    this.featureToggle = this.config.featureToggle;
  }
}

export interface SecurityConfig {
  clientId: string;
  authority: string;
  apiUrl: string;
  apiScope: string;
}

export interface FeatureToggle  { 
}
