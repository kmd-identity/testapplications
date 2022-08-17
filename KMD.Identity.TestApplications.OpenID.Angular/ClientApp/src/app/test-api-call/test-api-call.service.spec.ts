import { TestBed } from '@angular/core/testing';

import { TestApiCallService } from './test-api-call.service';

describe('TestApiCallService', () => {
  let service: TestApiCallService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TestApiCallService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
