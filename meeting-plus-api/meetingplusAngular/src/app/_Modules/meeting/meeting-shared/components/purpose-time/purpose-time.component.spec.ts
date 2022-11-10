import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PurposeTimeComponent } from './purpose-time.component';

describe('PurposeTimeComponent', () => {
  let component: PurposeTimeComponent;
  let fixture: ComponentFixture<PurposeTimeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PurposeTimeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PurposeTimeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
