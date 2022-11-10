import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MeetingHeadComponent } from './meeting-head.component';

describe('MeetingHeadComponent', () => {
  let component: MeetingHeadComponent;
  let fixture: ComponentFixture<MeetingHeadComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MeetingHeadComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MeetingHeadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
