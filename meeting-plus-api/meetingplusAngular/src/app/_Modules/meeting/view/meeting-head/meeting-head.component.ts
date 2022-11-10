import { Component, OnInit, Input } from "@angular/core";
import * as moment from "moment";

@Component({
  selector: "app-meeting-head",
  templateUrl: "./meeting-head.component.html",
  styleUrls: ["./meeting-head.component.scss"]
})
export class MeetingHeadComponent implements OnInit {
  
  @Input("meeting") _meeting: any;
  startTime: string;
  startDate: string;
  constructor() {}

  ngOnInit() {
    this.setTimeDate();
  }

  setTimeDate() {
    let meetingDate = moment(new Date(this._meeting.from));
    this.startDate = meetingDate.format("ll");
    this.startTime = meetingDate.format("HH:MM a");
  }


  isMeetingUpcomming() : boolean
  {

    return moment(this._meeting.from).isAfter(moment())
  }
  
  meetingType()
  {

    if(this.isMeetingUpcomming()) return 1; // upcomming
    else if (this.isMeetingPrevious() ) return 2; // Previous
    else return 0;
  }

  isMeetingPrevious() : boolean
  {
   return (!this._meeting.notEnded || (moment(this._meeting.to).isBefore(moment()) && !this._meeting.started));
  }
  
  public onZeroTrigger(event: any, meeting) {
    if (event == "reached zero") {
      // this.UpcomingMeetings = this.UpcomingMeetings.filter(x => x.id != meeting.id);
      // this.CurrentMeetings.push(meeting);
    }
  }
}
