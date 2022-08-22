import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserDelegationComponent } from './user-delegation.component';

describe('UserDelegationComponent', () => {
  let component: UserDelegationComponent;
  let fixture: ComponentFixture<UserDelegationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UserDelegationComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UserDelegationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
