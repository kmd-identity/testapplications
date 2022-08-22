import { Component, Input, OnInit } from '@angular/core';
import { ErrorService } from '../error.service';

@Component({
  selector: 'app-error',
  templateUrl: './error.component.html',
  styleUrls: ['./error.component.css']
})
export class ErrorComponent implements OnInit {

  public error: string = '';

  constructor(private errorService: ErrorService) { }

  ngOnInit(): void {
    this.errorService.errorMessage$.subscribe(errorMessage => this.error = errorMessage);
  }

}
