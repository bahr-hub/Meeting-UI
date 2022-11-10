import { Component, OnInit } from '@angular/core';
import { HomeService } from '../../services/home.service';
import { DataSource } from 'src/app/shared/models/data-source.model';
import { MeetingTypeEnum } from 'src/app/shared/enums/meeting-type.enum';
import { Response } from 'src/app/shared/models/response.model';
import { MeetingSignalRService } from '../../../../../shared/services/meeting-signal-r.service';
import { LoaderService } from 'src/app/shared/services/loader.service';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from 'src/app/_Modules/auth/services/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  meetingList = null;
  dataSource: DataSource = new DataSource();
  selectedDay: Date;
  meetingTasks = null;
  meetingSearch: string;;
  constructor(private homeService: HomeService, private signalRService: MeetingSignalRService, private loader: LoaderService, private activatedRoute: ActivatedRoute, private AuthService: AuthService) {
  }
  ngOnInit() {

    this.signalRService.startConnection();
    this.signalRService.addBroadcastStartMeetingListener();
    this.signalRService.addBroadcastAddTopicListener();
    this.signalRService.addBroadcastAddTaskListener();
    this.signalRService.addBroadcastParticipantJoinedListener();
    this.signalRService.addBroadcastAddParticipantListener();
    this.signalRService.addBroadcastEndMeetingListener();
    this.signalRService.addBroadcastDeleteMeetingListener();
    this.signalRService.addBroadcastDeleteParticipantListener();
    this.signalRService.addBroadcastClosedTask();
    this.signalRService.addBroadcastClosedTopic();
    this.signalRService.addBroadcastCreateMeeting();
    this.signalRService.addBroadcastUpdateMeeting();

    this.signalRService.getAddedTask()
      .subscribe(item => {
        if (!this.meetingTasks.find(x => x.id == item.meetingId)) {
          this.getMeetingTasks().subscribe(response => {
            //this.loader.unblock()
            this.meetingTasks = response.data;
          }, err => {
          });

        }
        else {
          let meetingTask = this.meetingTasks.filter(x => x.meetingTask.find(x => x.id === item.id));
          if (meetingTask == null || meetingTask.length <= 0) {
            this.meetingTasks.find(x => x.id == item.meetingId).meetingTask.push(item);
          }
        }
      }
      );
    this.signalRService.CloseTask()
      .subscribe(closedtask =>  {
        debugger;
        let meetingTask = this.meetingTasks.filter(x => x.meetingTask.find(x => x.id === closedtask.id));
        if (meetingTask != null && meetingTask.length > 0) {
          //  this.meetingTasks.filter(x => x.meetingTask.find(x => x.id === closedtask.id))[0].status = closedtask.status
          this.meetingTasks.find(x => x.id == closedtask.meetingId).meetingTask.find(y => y.id == closedtask.id).status = closedtask.status
        }
      }

    );

    this.signalRService.EndMeeting()
      .subscribe(item => {
        if (item) {

          let meetings = this.meetingList.find(x => x.type == MeetingTypeEnum.Current).meetings = this.meetingList.find(x => x.type == MeetingTypeEnum.Current).meetings.filter(x => x.id == item.id);
          meetings.type = MeetingTypeEnum.Previous;
          this.meetingList.find(x => x.type == MeetingTypeEnum.Previous).meetings.push(meetings[0]);
          //  this.CurrentMeetings = this.meetingList.filter(x => x.type==MeetingTypeEnum.Current && x.id != item.id);
          item.type = MeetingTypeEnum.Previous;
          this.meetingList.find(x => x.type == MeetingTypeEnum.Current).meetings = this.meetingList.find(x => x.type == MeetingTypeEnum.Current).meetings.filter(x => x.id != item.id);
        }
      });
    this.signalRService.DeleteMeeting()
      .subscribe(item => {

        this.meetingList.find(x => x.type == MeetingTypeEnum.Current).meetings = this.meetingList.find(x => x.type == MeetingTypeEnum.Current).meetings.filter(x => x.id != item.id);
        this.meetingList.find(x => x.type == MeetingTypeEnum.Upcoming).meetings = this.meetingList.find(x => x.type == MeetingTypeEnum.Upcoming).meetings.filter(x => x.id != item.id);
        this.meetingList.find(x => x.type == MeetingTypeEnum.Previous).meetings = this.meetingList.find(x => x.type == MeetingTypeEnum.Previous).meetings.filter(x => x.id != item.id);

      });

    this.signalRService.getAddedTopic()
      .subscribe(item => {

        this.meetingList.filter(x => x.meetings.find(x => x.id == item.fkMeetingId))[0].meetings.find(x => x.id == item.fkMeetingId).meetingTopic.push(item)
      });
    this.signalRService.getAddedParticipant()
      .subscribe(items => {

        this.meetingList.meetingParticipant = items
      });
    this.signalRService.getCreatedMeeting().subscribe(item =>
    {
      if (item.meetingParticipant.find(x => x.participantId == this.AuthService.currentUserValue.id))
      this.meetingList.find(x => x.type == MeetingTypeEnum.Upcoming).meetings.push(item);

    });
    this.signalRService.getUpdateddMeeting().subscribe(item => {
      if (item.meetingParticipant.find(x => x.participantId == this.AuthService.currentUserValue.id))

      {
        let OldMeeting = this.meetingList.find(x => x.type == MeetingTypeEnum.Upcoming).meetings.find(x => x.id == item.id);
        let Index = this.meetingList.find(x => x.type == MeetingTypeEnum.Upcoming).meetings.indexOf(OldMeeting);
        this.meetingList.find(x => x.type == MeetingTypeEnum.Upcoming).meetings[Index] = item;
      }
       

    });



    this.signalRService.getEmittedValue().subscribe(item => {
      if (item.meetingParticipant.find(x => x.participantId == this.AuthService.currentUserValue.id)) {
        let OldMeeting = this.meetingList.find(x => x.type == MeetingTypeEnum.Current).meetings.find(x => x.id == item.id);
        let Index = this.meetingList.find(x => x.type == MeetingTypeEnum.Current).meetings.indexOf(OldMeeting);
        this.meetingList.find(x => x.type == MeetingTypeEnum.Current).meetings[Index] = item;
      }


    });
    this.signalRService.getJoinedParticipant().subscribe(item => {
      if (item.participantId == this.AuthService.currentUserValue.id) {
        let OldPart = this.meetingList.find(x => x.type == MeetingTypeEnum.Current).meetings.find(x => x.id == item.meetingId).meetingParticipant.find(y=>y.id==item.id);
        let Index = this.meetingList.find(x => x.type == MeetingTypeEnum.Current).meetings.find(x => x.id == item.meetingId).meetingParticipant.indexOf(OldPart);
        this.meetingList.find(x => x.type == MeetingTypeEnum.Current).meetings.find(x => x.id == item.meetingId).meetingParticipant[Index] = item;
      }


    });



  }
  //ngOnInit() {
  //  this.signalRService.startConnection();
  //  this.signalRService.addBroadcastStartMeetingListener();
  //  this.signalRService.addBroadcastAddTopicListener();
  //  this.signalRService.addBroadcastAddTaskListener();
  //  this.signalRService.addBroadcastParticipantJoinedListener();
  //  this.signalRService.addBroadcastAddParticipantListener();
  //  this.signalRService.addBroadcastEndMeetingListener();
  //  this.signalRService.addBroadcastDeleteMeetingListener();
  //  this.signalRService.addBroadcastDeleteParticipantListener();
  //  this.signalRService.addBroadcastClosedTask();
  //  this.signalRService.addBroadcastClosedTopic();
  //  this.signalRService.getEmittedValue()
  //    .subscribe(item => {

  //      var current = this.meetingList.filter(x => x.type == MeetingTypeEnum.Current && x.id == item.id);
  //      if (current != null) {
  //        current = item;
  //      }
  //      else {
  //        var upcoming = this.meetingList.find(x => x.type == MeetingTypeEnum.Upcoming && x.id == item.id);
  //        if (upcoming != null) {
  //          upcoming = item;
  //          this.meetingList.push(upcoming);
  //        }
  //        else {
  //          var previous = this.meetingList.filter(x => x.type == MeetingTypeEnum.Previous && x.id == item.id);
  //          if (previous != null) {
  //            previous = item;
  //          }
  //        }
  //      }
  //    });
  //  this.signalRService.getAddedTask()
  //    .subscribe(item => {
  //      debugger;
  //      let meetingTask = this.meetingTasks.filter(x => x.meetingTask.find(x => x.id === item.id));


  //      if (meetingTask == null) {
  //        this.meetingTasks.find(x => x.id == item.meetingId).meetingTask.push(item);
  //      }
  //    });
  //  this.signalRService.getJoinedParticipant()
  //    .subscribe(item => {
  //      var current = this.meetingList.find(x => x.type == MeetingTypeEnum.Current).meetings.find(x=>x.id == item.meetingId);

  //      if (current != null) {
  //        current.meetingParticipant.find(x => x.id === item.id).joinedMeeting = item.joinedMeeting;
  //        current.meetingParticipant.find(x => x.id === item.id).joinedMeetingTime = item.joinedMeetingTime;
  //      }
  //      else {
  //        var upcoming = this.meetingList.find(x => x.type == MeetingTypeEnum.Upcoming).meetings.find(x=> x.id == item.id);
  //        if (upcoming != null) {
  //          upcoming.meetingParticipant.find(x => x.id === item.id).joinedMeeting = item.joinedMeeting;
  //          upcoming.meetingParticipant.find(x => x.id === item.id).joinedMeetingTime = item.joinedMeetingTime;
  //        }
  //      }
  //    });
  //  this.signalRService.EndMeeting()
  //    .subscribe(item => {
  //      if (item) {

  //        let meetings = this.meetingList.find(x => x.type == MeetingTypeEnum.Current).meetings = this.meetingList.find(x => x.type == MeetingTypeEnum.Current).meetings.filter(x => x.id == item.id);
  //        meetings.type = MeetingTypeEnum.Previous;
  //        this.meetingList.find(x => x.type == MeetingTypeEnum.Previous).meetings.push(meetings[0]);
  //        //  this.CurrentMeetings = this.meetingList.filter(x => x.type==MeetingTypeEnum.Current && x.id != item.id);
  //        item.type = MeetingTypeEnum.Previous;
  //        this.meetingList.find(x => x.type == MeetingTypeEnum.Current).meetings = this.meetingList.find(x => x.type == MeetingTypeEnum.Current).meetings.filter(x => x.id != item.id);
  //      }
  //    });
  //  this.signalRService.DeleteMeeting()
  //    .subscribe(item => {

  //      this.meetingList.find(x => x.type == MeetingTypeEnum.Current).meetings = this.meetingList.find(x => x.type == MeetingTypeEnum.Current).meetings.filter(x => x.id != item.id);
  //      this.meetingList.find(x => x.type == MeetingTypeEnum.Upcoming).meetings = this.meetingList.find(x => x.type == MeetingTypeEnum.Upcoming).meetings.filter(x => x.id != item.id);
  //      this.meetingList.find(x => x.type == MeetingTypeEnum.Previous).meetings = this.meetingList.find(x => x.type == MeetingTypeEnum.Previous).meetings.filter(x => x.id != item.id);

  //    });
  //  this.signalRService.CloseTask()
  //    .subscribe(closedtask => {

  //      this.meetingTasks.filter(x => x.meetingTask.find(x => x.id === closedtask.id).status = closedtask.status);
  //    });
  //}
  ////////////////
  private getAllMeetings() {
    var filter = {
      key: "From",
      value: new Date(this.selectedDay.toUTCString()),
    };

    var meetingSearchFilter = {
      key: "meetingName",
      value: this.meetingSearch
    };

    this.dataSource.filter = [];
    this.dataSource.filter.push(filter);
    this.dataSource.filter.push(meetingSearchFilter);
    //this.loader.block();
    return this.homeService.getAllMeetings(this.dataSource);
  }

  private getMeetingTasks() {
    var filter = {
      key: "From",
      value: new Date(this.selectedDay.toUTCString()),
    };
    this.dataSource.filter = [];
    this.dataSource.filter.push(filter);
    //this.loader.block();
    return this.homeService.getMeetingTasks(this.dataSource);
  }

  private prepareMeetingList(response) {
    this.meetingList = new Array<Response>();
    this.meetingList = response.data;
  }

  public onSelectDay(event) {
    this.selectedDay = event;
    this.getAllMeetings().subscribe(response => {
      //this.loader.unblock()
      this.prepareMeetingList(response);
    }, err => {
    });
    this.getMeetingTasks().subscribe(response => {
      //this.loader.unblock()
      this.meetingTasks = response.data;
    }, err => {
    });

  }

}
