import { TestBed } from '@angular/core/testing';

import { AuthenticationContext } from './authentication-context.service';

describe('AuthenticationContextService', () => {
  let service: AuthenticationContext;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AuthenticationContext);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
