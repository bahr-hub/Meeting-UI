import { TestBed } from '@angular/core/testing';

import { ViewMeetingService } from './view-meeting.service';

describe('ViewMeetingService', () => {
  let service: ViewMeetingService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ViewMeetingService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
