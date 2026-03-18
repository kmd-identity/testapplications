import { Component, OnInit } from '@angular/core';
import { TestApiCallService } from './test-api-call.service'; 
import { Observable } from 'rxjs';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-test-api-call',
  templateUrl: './test-api-call.component.html',
  styleUrls: ['./test-api-call.component.css'],
  standalone: false
})
export class TestApiCallComponent implements OnInit {
  apiResponse$: Observable<any>;

  constructor(private testApiCallService: TestApiCallService) { 
    this.apiResponse$ = this.testApiCallService.testApiResponse$.pipe(
      filter(response => response !== null)
    );
  }

  ngOnInit() { 
    this.testApiCallService.callTestApi();
  }

}
