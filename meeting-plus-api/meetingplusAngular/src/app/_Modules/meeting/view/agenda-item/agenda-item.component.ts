import { Component, OnInit, Input } from '@angular/core';
import { MeetingTopicService } from "../services/meeting-topic.service";
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-agenda-item',
  templateUrl: './agenda-item.component.html',
  styleUrls: ['./agenda-item.component.scss']
})
export class AgendaItemComponent implements OnInit {

  @Input('topic') _topic;
  @Input("startAt") _startAt;
  done:boolean;
  constructor(  private MeetingTopicService: MeetingTopicService , private toastr:ToastrService) { 
    this.done = false;
  }

  ngOnInit() {
  }
  closeTopic(id:any )
  {
    if (!this._startAt) 
    {
      this.toastr.error('ErrorMsg.meeting_not_started_yet');
     return}
     
    this.MeetingTopicService.CloseTopic(id).subscribe(response => {
      if (response.success) {
        this.toastr.success(response.message)
      }
      else {
        this.toastr.error(response.message)
      }
    }, err => {
  });
  }
  deleteTopic(id:any )
  {
    
    this.MeetingTopicService.DeleteTopic(id).subscribe(response => {
      if (response.success) {
        this.toastr.success(response.message)
      }
      else {
        this.toastr.error(response.message)
      }
    }, err => {
  });
  }
  
  toggleState()
  {
    this.done = !this.done
  }

}
