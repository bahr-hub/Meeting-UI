import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MeetingTaskService } from "../services/meeting-task.service";
import{TaskModel} from "../models/task.model";
import { ToastrService } from 'ngx-toastr';
import { MeetingSignalRService } from '../../../../shared/services/meeting-signal-r.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-action-items-list',
  templateUrl: './action-items-list.component.html',
  styleUrls: ['./action-items-list.component.scss']
})
export class ActionItemsListComponent implements OnInit {
  
  @Input("actionItems") _actionItems;
  @Input("participants") _participants;
  @Input("startAt") _startAt;
  actionItem: FormGroup;
  formToggled: boolean;
  formSubmited: boolean;
  model: TaskModel = new TaskModel;
  constructor(private fb: FormBuilder , private MeetingTaskService:MeetingTaskService,
    private toastr: ToastrService
    , private signalRService: MeetingSignalRService, public translate: TranslateService) {
    this.actionItem = this.fb.group({
      'actionName':['',Validators.required],
      'assigne':['',Validators.required],
      'dueDate':[null,Validators.required]
    })
   }

  ngOnInit() {



  }
  addTask()
  {
    if (!this._startAt) 
     {
       this.toastr.error('ErrorMsg.meeting_not_started_yet');
      return}
    if (this.actionItem.invalid) {
      return;
    }
    this.model.name = this.actionItem.value.actionName;
    this.model.AssigneeId = this.actionItem.value.assigne;
    this.model.DueDate = this.actionItem.value.dueDate;
    this.model.MeetingId = this._participants[0].meetingId;
    this.formSubmited  = true;
  this.MeetingTaskService.Add(this.model).subscribe(response => {
    if (response.success) {
      this.toastr.success(response.message);
      this.toogleForm();
      this.formSubmited  = false;
    }
    else {
      this.formSubmited  = false;
      this.toastr.error(response.message)
    }
  }, err => {
    this.formSubmited  = false;
});
  }
  toogleForm()
  {
    this.formToggled = !this.formToggled;
  }

}
