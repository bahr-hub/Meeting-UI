import { Component, OnInit, Input } from '@angular/core';
import { Response } from '../../../../../shared/models/response.model';
import { MeetingTypeEnum } from 'src/app/shared/enums/meeting-type.enum';
import { AbilityService } from 'src/app/shared/services/ability.service';

@Component({
  selector: 'app-meetings-list',
  templateUrl: './meetings-list.component.html',
  styleUrls: ['./meetings-list.component.scss']
})

export class MeetingsListComponent implements OnInit {
  public PaddingSpace = '   ';
  constructor( public ability: AbilityService) { }
  @Input('title') title: string;
  @Input('list') meetingList:any;//Getting of meeting list from parent based on meetign type
  ngOnInit() {
    
  }

  public getMeetingTypeName(meetingType: number) {
    switch (meetingType) {
      case 0:
        return "CURRENT MEETINGS";
      case 1:
        return "UPCOMMING MEETINGS";
      case 2:
        return "PREVIOUS MEETINGS";
    }

  }

  
  public onZeroTrigger(event: any, meeting) {
    
    if (event == "reached zero") {
      let meetings = this.meetingList.find(x => x.type == MeetingTypeEnum.Upcoming).meetings = this.meetingList.find(x => x.type == MeetingTypeEnum.Upcoming).meetings.filter(x => x.id == meeting.id);
      meetings.type = MeetingTypeEnum.Current;
      this.meetingList.find(x => x.type == MeetingTypeEnum.Current).meetings.push(meetings[0]);
      this.meetingList.find(x => x.type == MeetingTypeEnum.Upcoming).meetings = this.meetingList.find(x => x.type == MeetingTypeEnum.Upcoming).meetings.filter(x => x.id != meeting.id);
      //item.type = MeetingTypeEnum.Previous;    
    }
  }
}
