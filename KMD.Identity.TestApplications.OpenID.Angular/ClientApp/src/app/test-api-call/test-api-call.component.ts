import { Component, OnInit } from '@angular/core';
import { TestApiCallService } from './test-api-call.service';

@Component({
  selector: 'app-test-api-call',
  templateUrl: './test-api-call.component.html',
  styleUrls: ['./test-api-call.component.css'],
  standalone: false
})
export class TestApiCallComponent implements OnInit {
  apiResponse: any;

  constructor(private testApiCallService: TestApiCallService) { 
    this.testApiCallService.testApiResponse$.subscribe(response => {
      console.log('Subscription received:', response);
      this.apiResponse = response;
    });
  }

  async ngOnInit() { 
    await this.testApiCallService.callTestApi();
  }

}
