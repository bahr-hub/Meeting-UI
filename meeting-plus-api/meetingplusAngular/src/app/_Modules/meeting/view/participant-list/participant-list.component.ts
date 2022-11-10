import { Component, OnInit, Input,OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { MeetingOperationsService } from '../../meeting-operation/services/meeting-opertaions.service';
import { Observable, Subscription } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-participant-list',
  templateUrl: './participant-list.component.html',
  styleUrls: ['./participant-list.component.scss']
})
export class ParticipantListComponent implements OnInit,OnDestroy {
  @Input("participants") _participants;
  @Input("creatorId") _creatorId;
  @Input("meeting") _meeting;
  participantsSubscriber: Subscription = new Subscription();
  participants;
  participantForm: FormGroup;
  formOpened: boolean;
  constructor(private fb: FormBuilder,
    private meetingOperations: MeetingOperationsService,
    private toaster: ToastrService,
    public translate: TranslateService) {
      this.formOpened = false;

   }

  ngOnInit() {
    this.prepareForm();
  }

  ngOnDestroy()
  {
    //cleaning up the subscribers
    this.participantsSubscriber.unsubscribe(); //releasing the memory held by that subscriber
  }

  prepareForm()
  {

    this.participantForm = this.fb.group({
      participantId:["",[Validators.required]]
    });
      this.getParticipants();
  }

  getParticipants()
  {
   
    this.participantsSubscriber =  this.meetingOperations.getParticipants(this._meeting.from,this._meeting.to).subscribe(res =>{
      this.participants = res.data;
     
    });
  }

  get participantId()
  {
    return this.participantForm.get('participantId') as FormControl;
  }
  addParticipant()
  {
    
    if(this.participantForm.valid)
    {
      //handel the adding of the data
     
      let participants: Array<any> = new Array<any>();
      this.participantForm.get('participantId').value.forEach(element => {
        const{id}  = element;
        participants.push({participantId:id});
      });
      this.meetingOperations.addParticipants(this._meeting.id,participants).subscribe(
        res=>{
          this.toaster.success("Participants are added Successfully","Participants Added");
          this.participantForm.reset();
        },
        err=>{
          this.toaster.error("Error adding Participants Failed","Participants Addation Error");
          this.participantForm.reset();
        }
      )
    }
  }

}
