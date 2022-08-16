import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewApiResponseComponent } from './view-api-response.component';

describe('ViewApiResponseComponent', () => {
  let component: ViewApiResponseComponent;
  let fixture: ComponentFixture<ViewApiResponseComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ViewApiResponseComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewApiResponseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
