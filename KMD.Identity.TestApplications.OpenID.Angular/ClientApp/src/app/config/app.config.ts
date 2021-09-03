import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class AppConfig {

  private config: any = null;

  constructor(private http: HttpClient) {

  }

  public get(key: string) : string{
    
    return this.config[key];
  }

  public async ensureLoaded() {
    if (this.config == null) {
      await this.load();
    }
    return Promise.resolve(true);
  }

  private async load() {
    this.config = await this.http.get('config/auth').toPromise();
  }
}
