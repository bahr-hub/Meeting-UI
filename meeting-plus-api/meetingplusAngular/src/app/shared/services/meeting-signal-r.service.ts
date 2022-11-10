import { Injectable, Output, EventEmitter } from '@angular/core';
import * as signalR from "@aspnet/signalr";
import { Result } from './system-api';
import { AuthService } from 'src/app/_Modules/auth/services/auth.service';

import { environment } from '../../../environments/environment'


@Injectable({
  providedIn: 'root'
})
export class MeetingSignalRService {
  public startMeetingResult: Result;
  public deleteMeetingResult: Result;
  public endMeetingResult: Result;
  public addTaskResult :Result;
  public addTopicResult:Result;
  public joinedParticipantResult:Result;
  public addParticipantResult:Result;
  public deleteParticipantResult:Result;
  public meetingID:string;
  public CloseTaskResult:Result;
  public CloseTopicResult:Result;
  public AddNoteResult: Result;
  public CreatMeetingResult: Result;
  public UpdateMeetingResult: Result;
  private hubConnection: signalR.HubConnection;

  @Output() fire: EventEmitter<any> = new EventEmitter();
  @Output() fireTopic: EventEmitter<any> = new EventEmitter();
  @Output() fireTask: EventEmitter<any> = new EventEmitter();
  @Output() fireJoinedParticipant: EventEmitter<any> = new EventEmitter();
  @Output() fireAddParticipant: EventEmitter<any> = new EventEmitter();
  @Output() fireEndMeeting: EventEmitter<any> = new EventEmitter();
  @Output() fireDeleteMeeting: EventEmitter<any> = new EventEmitter();
  @Output() fireDeleteParticipant: EventEmitter<any> = new EventEmitter();
  @Output() fireClosedTask:EventEmitter<any> = new EventEmitter();
  @Output() fireClosedTopic:EventEmitter<any> = new EventEmitter();
  @Output() fireAddNote: EventEmitter<any> = new EventEmitter();
  @Output() fireCreatedMeeting: EventEmitter<any> = new EventEmitter();
  @Output() fireUpdatedMeeting: EventEmitter<any> = new EventEmitter();
  private baseUrl: string;

  constructor(private AuthService: AuthService) {
    this.baseUrl = environment.apiUrl ? environment.apiUrl : "http://localhost:6251";
  }
  public startConnection = () => {


    const options: signalR.IHttpConnectionOptions = {
      accessTokenFactory: () => {
        return this.AuthService.currentUserValue.token;
      }
    };
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.baseUrl + 'MeetingHub', options)
      .build();
    this.hubConnection.onclose(async () => {
      await this.start();
    });
    this.hubConnection
      .start()
      .then(() => console.log('Connection started on MeetingHub signalr'))
      .catch(err => console.log('Error while starting connection 111: ' + err));
  }


  async start() {
    try {
      await this.hubConnection.start();
   
    } catch (err) {
      
      setTimeout(() => this.start(), 5000);
    }
  };

 
  public addBroadcastStartMeetingListener = () => {
    this.hubConnection.on('startMeeting', (data) => {
      this.startMeetingResult = data;

      if(this.startMeetingResult.success==true)
      this.fire.emit(this.startMeetingResult.data);

    })
  }
  public addBroadcastAddTopicListener = () => {
    this.hubConnection.on('addTopic', (data) => {
      this.addTopicResult = data;

      if(this.addTopicResult.success==true)
      this.fireTopic.emit(this.addTopicResult.data);

    })
  }

  public addBroadcastAddTaskListener = () => {
    
    this.hubConnection.on('addTask', (data) => {
      this.addTaskResult = data;

      if(this.addTaskResult.success==true)
      this.fireTask.emit(this.addTaskResult.data);

    })
  }
   
  public addBroadcastParticipantJoinedListener = () => {
    this.hubConnection.on('joinMeeting', (data) => {
      this.joinedParticipantResult = data;

      if(this.joinedParticipantResult.success==true)
      this.fireJoinedParticipant.emit(this.joinedParticipantResult.data);

    })
  }

  public addBroadcastAddParticipantListener = () => {
    this.hubConnection.on('addParticipant', (data) => {
      this.addParticipantResult = data;

      if(this.addParticipantResult.success==true)
      this.fireAddParticipant.emit(this.addParticipantResult.data);

    })
  }
  public addBroadcastEndMeetingListener = () => {
    this.hubConnection.on('endMeeting', (data) => {
      this.endMeetingResult = data;

      if(this.endMeetingResult.success==true)
      this.fireEndMeeting.emit(this.endMeetingResult.data);

    })
  }
  public addBroadcastDeleteMeetingListener = () => {
    this.hubConnection.on('deleteMeeting', (data) => {
      this.deleteMeetingResult = data;

      if(this.deleteMeetingResult.success==true)
      this.fireDeleteMeeting.emit(this.deleteMeetingResult.data);

    })
  }
  public addBroadcastDeleteParticipantListener = () => {
    this.hubConnection.on('deleteParticipant', (data) => {
      this.deleteParticipantResult = data;

      if(this.deleteParticipantResult.success==true)
      this.fireDeleteParticipant.emit(this.deleteParticipantResult.data);

    })
  }
  public addBroadcastClosedTask= () => {
    this.hubConnection.on('closeTask', (data) => {
      this.CloseTaskResult = data;

      if(this.CloseTaskResult.success==true)
      this.fireClosedTask.emit(this.CloseTaskResult.data);

    })
  }
  public addBroadcastClosedTopic= () => {
    this.hubConnection.on('closeTopic', (data) => {
      this.CloseTopicResult= data;

      if(this.CloseTopicResult.success==true)
      this.fireClosedTopic.emit(this.CloseTopicResult.data);

    })
  }
  public addBroadcastAddNote= () => {
    this.hubConnection.on('addNote', (data) => {
      this.AddNoteResult= data;

      if(this.AddNoteResult.success==true)
      this.fireAddNote.emit(this.AddNoteResult.data);

    })
  }
  public addBroadcastCreateMeeting = () => {
    this.hubConnection.on('createMeeting', (data) => {
      this.CreatMeetingResult = data;

      if (this.CreatMeetingResult.success == true)
        this.fireCreatedMeeting.emit(this.CreatMeetingResult.data);

    })
  }
  public addBroadcastUpdateMeeting = () => {
    this.hubConnection.on('updateMeeting', (data) => {
      this.UpdateMeetingResult = data;

      if (this.UpdateMeetingResult.success == true)
        this.fireUpdatedMeeting.emit(this.UpdateMeetingResult.data);

    })
  }
  getEmittedValue() {
    
    return this.fire;
  }

  getAddedTopic ()
  {
     return this.fireTopic;
  }
  getCreatedMeeting() {
    return this.fireCreatedMeeting;
  }
  getUpdateddMeeting() {
    return this.fireUpdatedMeeting;
  }
  getAddedTask ()
  {
    
     return this.fireTask;
  }
  getJoinedParticipant ()
  {
    return this.fireJoinedParticipant;
  }
  getAddedParticipant ()
  {
    return this.fireAddParticipant;
  }
  getDeletedParticipant ()
  {
    return this.fireDeleteParticipant;
  }
  EndMeeting ()
  {
    return this.fireEndMeeting;
  }
  DeleteMeeting()
  {
    return this.fireDeleteMeeting;
  }
  CloseTask()
  {
    return this.fireClosedTask;
  }
  CloseTopic()
  {
    return this.fireClosedTopic;
  }
  getAddedNote()
  {
    return this.fireAddNote;
  }
}
