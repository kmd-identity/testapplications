import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ErrorService {

  private errorMessage = new BehaviorSubject<any>('');
  public errorMessage$ = this.errorMessage.asObservable();

  private isError = new BehaviorSubject<boolean>(false);
  public isError$ = this.isError.asObservable();

  constructor() { }

  public reset(): void {
    this.errorMessage.next('')
    this.isError.next(false);
  }

  public raise(errorMessage: any): void {
    this.errorMessage.next(errorMessage);
    this.isError.next(true);
  }
}
