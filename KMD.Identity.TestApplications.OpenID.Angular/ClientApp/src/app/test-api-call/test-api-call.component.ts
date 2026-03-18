import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { TestApiCallService } from './test-api-call.service';

@Component({
  selector: 'app-test-api-call',
  templateUrl: './test-api-call.component.html',
  styleUrls: ['./test-api-call.component.css'],
  standalone: false
})
export class TestApiCallComponent implements OnInit {
  apiResponse: any;

  constructor(
    private testApiCallService: TestApiCallService,
    private cdr: ChangeDetectorRef
  ) { 
    this.testApiCallService.testApiResponse$.subscribe(response => {
      console.log('Subscription received:', response);
      this.apiResponse = response;
      if (response) {
        this.cdr.detectChanges(); 
      }
     
    });
  }

  ngOnInit() { 
    this.testApiCallService.callTestApi();
  }

}
