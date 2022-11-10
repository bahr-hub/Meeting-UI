import { Component, OnInit } from "@angular/core";
import { ViewMeetingService } from "../services/view-meeting.service";
import { ActivatedRoute, Router } from "@angular/router";
import { ToastrService } from 'ngx-toastr';
import { MeetingSignalRService } from '../../../../shared/services/meeting-signal-r.service';
//import { LoaderService } from 'src/app/shared/services/loader.service';
import { debugOutputAstAsTypeScript } from '@angular/compiler';
import { AuthService } from 'src/app/_Modules/auth/services/auth.service';
import { TranslateService } from '@ngx-translate/core';



@Component({
  selector: "app-view",
  templateUrl: "./view.component.html",
  styleUrls: ["./view.component.scss"]
})
export class ViewComponent implements OnInit {
  currentMeeting: any;
  isAnonymous: boolean = false;

  constructor(private toastr: ToastrService,
    private meetingService: ViewMeetingService,
    router: ActivatedRoute,
    private signalRService: MeetingSignalRService,
    //private loader:LoaderService ,
    private route: ActivatedRoute,
    private _router: Router,
    private authService: AuthService,
    public translate: TranslateService

  ) {
    this.route.queryParams.subscribe((params) => {
      debugger;
      let access_token = params["access_token"];
      if (access_token) {
        this.isAnonymous = true;
        // this.userService.getByToken()
        // .subscribe((response) => {

        //   console.log(response);
        // });
        this.authService.setToken(JSON.stringify(access_token));
        this.authService.guestLogin()
          .subscribe((response) => {

            if (response.success) {
              this.authService.setCurrentUser(JSON.stringify(response.data));
              let meetingId = params["id"];
              if (meetingId) {
                this.loadMeeting(meetingId);
              }

            }
          });

      }
      else {
        let meetingId = this.route.snapshot.params["id"];
        if (meetingId) {
          this.loadMeeting(meetingId);
        }
      }
    });

  }

  private loadMeeting(id) {
    //this.loader.block();
    this.meetingService.get(id).subscribe(_meeting => {

      this.currentMeeting = _meeting.data;
      //this.loader.unblock();
    });
  }
  isRunningMeeting(): boolean {
    if (!this.currentMeeting.startedAt) {
      this.toastr.error('ErrorMsg.meeting_not_started_yet');
      return false;
    }
    return true;
  }

  addNote() {

    if (!this.isRunningMeeting()) {
      return;
    }

    this.meetingService.addNote(this.currentMeeting.id, this.currentMeeting.notes).subscribe(response => {
      console.log('Success');
    }, err => {
      console.log("ERROR");
    });
  }

  ngDestroy() {
    console.log("Destroyed")
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
    this.signalRService.addBroadcastUpdateMeeting();
    this.signalRService.getEmittedValue()
      .subscribe(item => this.currentMeeting = item);
    this.signalRService.getAddedTask()
      .subscribe(item => {

        let meetingTask = this.currentMeeting.meetingTask.find(x => x.id === item.id);
        if (meetingTask == null) {
          this.currentMeeting.meetingTask.push(item);
        }
      });


    this.signalRService.getAddedTopic()
      .subscribe(item => this.currentMeeting.meetingTopic.push(item));
    this.signalRService.getJoinedParticipant()
      .subscribe(item => {
        this.currentMeeting.meetingParticipant.find(x => x.id === item.id).joinedMeeting = item.joinedMeeting;
        this.currentMeeting.meetingParticipant.find(x => x.id === item.id).joinedMeetingTime = item.joinedMeetingTime;
      })
    this.signalRService.getAddedParticipant()
      .subscribe(items => this.currentMeeting.meetingParticipant = items)
    this.signalRService.EndMeeting()
      .subscribe(item => {

        this.currentMeeting = item;
      })
    this.signalRService.DeleteMeeting()
      .subscribe(item => {

        this._router.navigate(['/Meetings']);
      }
      )
    this.signalRService.getDeletedParticipant()
      .subscribe(items => {

        this.currentMeeting.meetingParticipant = items;
      })
    this.signalRService.CloseTask()
      .subscribe(closedtask => {
        this.currentMeeting.meetingTask.find(x => x.id === closedtask.id).status = closedtask.status;
      }

      )
    this.signalRService.CloseTopic()
      .subscribe(closedTopic => {

        this.currentMeeting.meetingTopic.find(x => x.id === closedTopic.id).isClosed = closedTopic.isClosed;
      }

    )
    this.signalRService.getUpdateddMeeting().subscribe(item => {
      if (item.meetingParticipant.find(x => x.participantId == this.authService.currentUserValue.id) && item.id == this.currentMeeting.id) {
        this.currentMeeting = item;
      }


    });

  }
}
