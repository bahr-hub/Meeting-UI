import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MeetingGattingComponent } from './meeting-gatting.component';

describe('MeetingGettingComponent', () => {
  let component: MeetingGattingComponent;
  let fixture: ComponentFixture<MeetingGattingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [MeetingGattingComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MeetingGattingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
