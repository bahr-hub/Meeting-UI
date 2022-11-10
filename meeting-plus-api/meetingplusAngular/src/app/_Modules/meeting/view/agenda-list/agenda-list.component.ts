import { Component, OnInit, Input } from "@angular/core";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { MeetingTopicService } from "../services/meeting-topic.service";
import { TopicModel } from "../models/topic.model";
import { ToastrService } from 'ngx-toastr';
import { TranslateService } from '@ngx-translate/core';


@Component({
  selector: "app-agenda-list",
  templateUrl: "./agenda-list.component.html",
  styleUrls: ["./agenda-list.component.scss"]
})
export class AgendaListComponent implements OnInit {
  @Input("agendaItems") _agendaItems;
  @Input("participants") _participants;
  @Input("startAt") _startAt;

  topicForm: FormGroup;
  toggledForm: boolean;
  topics: any;
  model: TopicModel = new TopicModel;
  public disableAddTopic: boolean = false;//To prevent user to add topic twice
  constructor(private fb: FormBuilder, private MeetingTopicService: MeetingTopicService
    , private toastr: ToastrService
    , public translate: TranslateService) {
    this.toggledForm = false;
    this.topicForm = this.fb.group({
      topicName: ["", Validators.required],
      presenter: [null, Validators.required],
      duration: ["", [Validators.required, Validators.min(1)]]
    });
    this.topics = [];
  }
  toggleTopicForm() {
    this.toggledForm = !this.toggledForm;
  }

  addTopic() {
    if (this.topicForm.invalid) {
      return;
    }
    this.model.name = this.topicForm.value.topicName;
    this.model.presenterId = this.topicForm.value.presenter;
    this.model.duration = this.topicForm.value.duration;
    this.model.fkMeetingId = this._participants[0].meetingId;
    this.disableAddTopic = true;
    this.MeetingTopicService.Add(this.model).subscribe(response => {
      this.handelResponse(response);
    }, err => {
      this.disableAddTopic = false;
    });

  }
  handelResponse(response) {
    if (response.success) {
      this.toastr.success(response.message);
      this.toggleTopicForm();
    }
    else {
      this.toastr.error(response.message);
    }
    this.disableAddTopic = false;
  }
  ngOnInit() {
  }
}
