import { TestBed } from '@angular/core/testing';

import { UserDelegationService } from './user-delegation.service';

describe('UserDelegationService', () => {
  let service: UserDelegationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UserDelegationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
