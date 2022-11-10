import { Component, OnInit, Input, ViewChild } from '@angular/core';
import * as moment from 'moment';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-meeting-card',
  templateUrl: './meeting-card.component.html',
  styleUrls: ['./meeting-card.component.scss']
})
export class MeetingCardComponent implements OnInit {
  @Input('meeting') meeting;
  @Input() meetingType: number;
  public isTopicsToggled: boolean;

  constructor(public translate: TranslateService) {
    this.isTopicsToggled = false;
    
  }

  ngOnInit() {
  }

  toggleTopics() {
    this.isTopicsToggled = !this.isTopicsToggled;
  }
  


  public onZeroTrigger(event: any, meeting) {

    if (event == "reached zero") {
// console.log(meeting);
//       this.meetingType = 0;
//       meeting.type = 0;
      // this.UpcomingMeetings = this.UpcomingMeetings.filter(x => x.id != meeting.id);
      // this.CurrentMeetings.push(meeting);
    }
  }

}
