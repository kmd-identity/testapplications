import { TestBed } from '@angular/core/testing';

import { AuthenticationContextService } from './authentication-context.service';

describe('AuthenticationContextService', () => {
  let service: AuthenticationContextService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AuthenticationContextService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
