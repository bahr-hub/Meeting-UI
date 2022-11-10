import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray, FormControl } from '@angular/forms';
import { MeetingOperationsService } from '../services/meeting-opertaions.service';
import { ProjectService } from 'src/app/_Modules/lockups/project/services/project.service';
import { DataSource } from 'src/app/shared/models/data-source.model';
import { TagService } from 'src/app/_Modules/lockups/tag/services/tag.service';
import { ToastrService } from 'ngx-toastr';
import * as moment from 'moment';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from 'src/app/_Modules/auth/services/auth.service';
import { TranslateService } from '@ngx-translate/core';
import { LoaderService } from 'src/app/shared/services/loader.service';
import { MeetingModel } from 'src/app/_Modules/meeting/meeting-shared/models/meeting.model';
import { Response } from 'selenium-webdriver/http';
import { ConfirmationModalComponent } from 'src/app/shared/components/confirmation-dialog/confirmation-modal.component';

@Component({
  selector: 'app-operation',
  templateUrl: './operation.component.html',
  styleUrls: ['./operation.component.scss']
})
export class OperationComponent implements OnInit {

  //#region [31-3-2020]-[Mahmoud Ali]-[Variables Declarations] 
  meetingForm: FormGroup;
  topicForm: FormGroup;
  projects: any;
  locations: any;
  participants: any;
  tags: Array<any>;
  editMode: boolean;
  meeting: MeetingModel;
  formSubmited: boolean;
  topicFormSubmited: boolean;
  prevMeetings = [];
  meetingId: any = null;
  @ViewChild(ConfirmationModalComponent) confirmationModalComponent;//This open dialog for confirmation of deleting Meeting Agenda
  tmpDateFrom: Date;
  tmpDateTo: Date;

  //#endregion

  //#region [31-3-2020]-[Mahmoud Ali]-[Meeting component Life cycle]
  constructor(
    private fb: FormBuilder,
    private meetingService: MeetingOperationsService,
    private projectService: ProjectService,
    private tagService: TagService,
    private toastService: ToastrService,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute,
    public translate: TranslateService,
    private loader: LoaderService) {
    this.meetingId = this.route.snapshot.paramMap.get("id");
    this.prepareDropDownLists();
    this.meetingId ? this.editMode = true : this.editMode = false;
    this.meetingId ? this.getMeetingById(this.meetingId) : this.prepareForm();
    this.formSubmited = false;

  }

  ngOnInit(): void {
  }
  //#endregion

  //#region  [31-3-2020]-[Mahmoud Ali]-[Get Meeting By Id in Edit Mode]
  private getMeetingById(id) {
    this.meetingService.get(id).subscribe(res => {
      this.meeting = res.data;
      this.setTempDates();//if user change date and he cancel the dropping of meeting topics
      this.loadParticipants(res.data.from, res.data.to, true);
      this.loadLocations(res.data.from, res.data.to, true);
      if (res.data.meetingParticipant && res.data.meetingParticipant.length > 0) {
        res.data.meetingParticipant.forEach(participant => {
          participant['name'] = participant.participant.name
        });
      }
      if (res.data.meetingTag && res.data.meetingTag.length > 0) {
        res.data.meetingTag.forEach(tag => {
          tag['name'] = this.tags.filter(t => t.tagId = tag.tagId)[0].name
        });
      }
      this.prepareForm(res.data);
    })
  }
  //#endregion

  //#region [31-3-2020]-[Mahmoud Ali]-[Preparing of Meeting Form]
  private prepareForm(meetingData?: any) {
    this.meetingForm = this.fb.group({
      name: [meetingData ? meetingData.name : "", [Validators.required, Validators.maxLength(50)]],
      from: [meetingData ? meetingData.from : null, [Validators.required]],
      to: [meetingData ? meetingData.to : null, [Validators.required]],
      projectId: [meetingData ? meetingData.projectId : null, [Validators.required]],
      locationId: [meetingData ? meetingData.locationId : null, [Validators.required]],
      previousMeetingID: [meetingData ? meetingData.previousMeetingID : null],
      meetingTag: [meetingData ? meetingData.meetingTag : null],
      meetingParticipant: [meetingData ? meetingData.meetingParticipant : null, [Validators.required]],
      meetingTopic: this.fb.array(meetingData ? meetingData.meetingTopic : [], [Validators.required]),
      status: [meetingData ? meetingData.status : 0]
    });
    this.topicForm = this.fb.group({
      name: ["", [Validators.required, Validators.maxLength(50)]],
      presenterId: [null, [Validators.required]],
      presenterName: [null, [Validators.required]],
      duration: ["", [Validators.required, Validators.min(1)]]
    });
    this.onChanges();
  }

  private setTempDates() {
    this.tmpDateFrom = this.meeting.from;
    this.tmpDateTo = this.meeting.to;
  }
  //#endregion 

  //#region  [31-3-2020]-[Mahmoud Ali]-[Prepaing dropdownlist in meeting form] 
  private prepareDropDownLists() {
    this.getProjects();
    this.getTags();
    this.getAllMeetings();
  }
  //Bind Projects
  private getProjects() {
    let datasource = new DataSource();
    datasource.pageSize = 10000;
    this.projectService.getAll(datasource)
      .subscribe(response => this.projects = response.data);
  }
  //Bind Follow up meetings
  getAllMeetings() {
    this.meetingService.getAllLite()
      .subscribe(response => {
        this.prevMeetings = response.data
      }
      );
  }
  //Bind Tags
  private getTags() {
    let datasource = new DataSource();
    datasource.pageSize = 10000;
    this.tagService.getAll(datasource)
      .subscribe(response => {
        let data = [];
        response.data.forEach(tag => {
          const { id, name } = tag;
          data.push({ "tagId": id, "name": name });
        });
        this.tags = data;
      });
  }
  //Bind Locations
  private loadLocations(start, end, isEditMode?: boolean) {

    let datasource = new DataSource();
    datasource.pageSize = 10000;
    this.meetingService.getLocations(start, end, this.meetingId)
      .subscribe(response => {
        this.locations = response.data;
        if (!isEditMode)//Is not in edit mode
          this.meetingForm.get('locationId').setValue(undefined);
      });
  }
  //Bind Participants
  private loadParticipants(start, end, isEditMode?: boolean) {
    this.meetingService
      .getParticipants(start, end, this.meetingId)
      .subscribe(res => {
        let data = [];
        res.data.forEach(participant => {
          delete Object.assign(participant, { ["participantId"]: participant["id"] })["id"];
          const { participantId, name } = participant;
          data.push({ participantId, name });
        });
        this.participants = data;
        if (this.isMeetingCreatorBusy()) return;
        const { id, name } = this.authService.currentUserValue;
        const currentParticipant = { "participatnId": id, "name": name };
        if (!isEditMode || !this.meetingForm.get("meetingParticipant").value) {
          this.meetingForm.get("meetingParticipant").setValue([currentParticipant]);
        }

      });
  }

  //#endregion

  //#region  [31-3-2020]-[Mahmoud Ali]-[Handling of Topic area]
  //Add New topic
  addTopic() {
    this.topicFormSubmited = true;
    this.validateDuration();
    if (!this.meetingHasParticipants()) {
      return;
    }
    if (this.topicForm.invalid) {
      // this.toastService.error(this.translate.instant("please check all the fields"), this.translate.instant("topic creation failed"));
      return;
    }
    const topic = this.fb.group({
      name: [this.topicForm.value.name],
      presenterId: [this.topicForm.value.presenterId],
      duration: [this.topicForm.value.duration],
      presenterName: [this.topicForm.value.presenterName],
    });
    this.topicsArray.push(topic);
    this.topicForm.reset();
    this.topicFormSubmited = false;
  }

  //Remove topic
  removeTopic(i) {
    if (this.topicsArray.length > 0)
      this.topicsArray.removeAt(i);
  }
  //Set Participant Name
  setPresenterName(id) {
    let participants = this.meetingForm.get('meetingParticipant').value;
    if (participants.length > 0 && id != (null || undefined || '')) {
      let presenterName = participants.filter(x => x.participantId === id)[0].name;
      this.topicForm.get('presenterName').setValue(presenterName);
      this.topicForm.get('presenterId').setValue(id);
    }

  }

  //Topics Validation
  validateDuration() {
    let start = moment(this.to.value);
    let end = moment(this.from.value);
    let duration = moment.duration(start.diff(end)).asMinutes();
    if (!duration || !this.topicForm.get('duration').value) {
      return false;
    }
    let total: number = 0;
    for (let i = 0; i < this.topicsArray.controls.length; i++) {
      total += parseInt(this.topicsArray.controls[i].value.duration);
    }
    total += parseInt(this.topicForm.get('duration').value);
    if (total > duration) {
      this.topicForm.get('duration').setErrors({ duration: true });
      return false;
    } else {
      this.topicForm.get('duration').setErrors(null);
      return true;
    }


  }

  validateMeetingTopics() {
    if (this.topicsArray && this.topicsArray.value.length > 0) {
      this.openDialog();
    }
    else {
      this.onDateChanges();
    }
  }

  //Getting of topic properties 
  get topicsArray() {
    return this.meetingForm.get('meetingTopic') as FormArray;
  }

  get topicName() {
    return this.topicForm.get('name') as FormControl;
  }

  get duration() {
    return this.topicForm.get('duration') as FormControl;
  }

  get presenterId() {
    return this.topicForm.get('presenterId') as FormControl;
  }
  //#endregion

  //#region [31-3-2020]-[Mahmoud Ali]-[Getting of Meeting Properties]
  get meetingName() {
    return this.meetingForm.get('name') as FormControl;
  }

  get from() {
    return this.meetingForm.get('from') as FormControl;
  }

  get to() {
    return this.meetingForm.get('to') as FormControl;
  }

  get projectId() {
    return this.meetingForm.get('projectId') as FormControl;
  }

  get locationId() {
    return this.meetingForm.get('locationId') as FormControl;
  }

  get previousMeetingID() {
    return this.meetingForm.get('previousMeetingID') as FormControl;
  }

  get meetingTag() {
    return this.meetingForm.get('meetingTag') as FormControl;
  }

  get meetingParticipant() {
    return this.meetingForm.get('meetingParticipant') as FormControl;
  }


  //#endregion

  //#region [31-3-2020]-[Mahmoud Ali]-[Saving of Meeting(Add/Update)]
  saveMeeting() {
    this.formSubmited = true;

    if (this.topicsArray.length == 0) {
      this.toastService.error(this.translate.instant("cannot create a meeting without topics"));
      return;
    }

    if (!this.meetingForm.valid) {
      return;
    }
    // this.meetingForm.get('from').setValue(moment(this.meetingForm.get('from').value).toISOString());
    // this.meetingForm.get('to').setValue(moment(this.meetingForm.get('to').value).toISOString());
    let meetingTemp = this.meeting;
    this.meeting = this.meetingForm.value;
    if (meetingTemp?.createdBy)
      this.meeting.createdBy = meetingTemp.createdBy;
    this.meeting.from = new Date(this.meetingForm.get('from').value);
    this.meeting.to = new Date(this.meetingForm.get('to').value);

    if (!this.validateMeetingDates()) {//It will validate if date from is greater than date to and meeting is in past or not 
      return;
    }

    if (!this.meetingHasParticipants()) {
      return;
    }



    if (this.editMode) {
      this.loader.block();
      this.meetingService.update(this.setMeetingForUpdate(this.meeting))
        .subscribe(res => {
          this.loader.unblock();
          if (res["success"]) {
            this.toastService.success(this.translate.instant("meeting Updated successfully", "Meeting Updated"));
            this.router.navigateByUrl(`/Meetings/view/${this.route.snapshot.paramMap.get("id")}`);
          }
          else {
            this.toastService.error(this.translate.instant(res["message"]));
          }
        },
        err => {
          this.handleResponse();
        }
        );
    }
    else {
      this.loader.block();
      this.meetingService.create(this.meeting)
        .subscribe(response => {
          if (response["success"]) {
            this.loader.unblock();
            this.toastService.success(this.translate.instant("meeting created successfully", "Meeting Created"));
            this.router.navigateByUrl(`/Meetings/view/${response["data"].id}`);
          }
          else {
            this.toastService.error(this.translate.instant(response["message"]));
          }
        }
        ,
        err => {
          this.handleResponse();
        });
    }


    // }

    // else {
    //    this.toastService.error(this.translate.instant("please check all the fields"), this.translate.instant("Meeting creation failed"));
    // }
  }



  private handleResponse() {
    this.loader.unblock();
  }

  private setMeetingForUpdate(meeting) {
    //participants
    let _participants = [];
    let _tags = [];
    let _topics = [];
    meeting.meetingParticipant.forEach(participant => {
      _participants.push({ participantId: participant.participantId });
    });
    meeting.meetingParticipant = _participants;
    meeting.meetingTag.forEach(tag => {
      _tags.push({ tagId: tag.tagId });
    });
    meeting.meetingTag = _tags;
    meeting.meetingTopic.forEach(topic => {
      _topics.push({
        name: topic.name,
        presenterId: topic.presenterId,
        duration: topic.duration
      });
    });
    meeting.meetingTopic = _topics;
    meeting["id"] = this.route.snapshot.paramMap.get("id");
    // meeting["createdBy"] = this.meeting.createdBy;
    return meeting;
  }
  //#endregion

  //#region [31-3-2020]-[Mahmoud Ali]-[Handling of Validation]

  validateMeetingDates() {
    //If Meeting in the paste
    if (new Date(this.meetingForm.get('from').value) < new Date()) {
      this.toastService.error(this.translate.instant("meeting cant be started in the past"));
      return false;
    }
    //If Date From is greater than Date To
    if (new Date(this.meetingForm.get('from').value) > new Date(this.meetingForm.get('to').value)) {
      this.toastService.error(this.translate.instant("meeting start date cannot exceed meeting end date"));
      return false;
    }
    return true;
  }

  isMeetingCreatorBusy() {
    let creatorId = this.authService.currentUserValue.id;
    let meetingCreator = this.participants.find(x => x.participantId == creatorId);
    if (meetingCreator) {
      return false;
    }
    else {
      this.toastService.error(this.translate.instant("meeting.create_meeting.creator_busy"));
      this.participants = [];
      return true;
    }
  }
  validateUser(participant) {
    if (this.authService.currentUserValue.id === participant.value.participantId) {
      var selectedParticipants: Array<any> = this.meetingParticipant.value;
      selectedParticipants.unshift(participant.value);
      this.meetingParticipant.setValue(selectedParticipants);
      this.toastService.error(this.translate.instant("Cannot Remove the meeting creator"));
    }
    else if (this.topicsArray.value.filter(x => x.presenterId == participant.value.participantId).length > 0) {
      var selectedParticipants: Array<any> = this.meetingParticipant.value;
      selectedParticipants.unshift(participant.value);
      this.meetingParticipant.setValue(selectedParticipants);
      this.toastService.error(this.translate.instant("Cannot Remove a participant that is assigned to a topic"));
    }
  }

  meetingHasParticipants() {
    if (this.meetingParticipant.value && this.meetingParticipant.value.length > 1) {
      return true;
    }
    else {
      this.toastService.error(this.translate.instant("meeting.create_meeting.has_not_participants"));
      return false;
    }
  }
  //#endregion

  //#region  [31-3-2020]-[Mahmoud Ali]-[Dates Chaning(From/To)]
  onChanges() {
    this.meetingForm.get('from').valueChanges.subscribe(val => {
      this.validateMeetingTopics();
    });

    this.meetingForm.get('to').valueChanges.subscribe(val => {
      this.validateMeetingTopics();
    });
  }

  private onDateChanges() {
    let startDate = this.meetingForm.get('from').value;
    let endDate = this.meetingForm.get('to').value;
    this.tmpDateFrom = startDate;//resting of date from 
    this.tmpDateTo = endDate;//reseting of date to
    if (!startDate || !endDate || !this.validateMeetingDates()) {
      return;
    }

    this.loadLocations(moment(startDate).toISOString(), moment(endDate).toISOString());
    this.loadParticipants(moment(startDate).toISOString(), moment(endDate).toISOString());
  }
  //#endregion

  openDialog(): void {
    this.confirmationModalComponent.open(null, "You will drop any data in Meeting Agenda");//Open dialog when you press in delete button
  }

  cancelDroppingMeetingTopics(event) {
    this.meetingForm.get('from').setValue(this.tmpDateFrom);
    this.meetingForm.get('to').setValue(this.tmpDateTo);
  }

  public dropMeetingTopics(event) {
    this.onDateChanges();
    this.topicsArray.controls = [];
    this.topicsArray.setValue([]);

    if (this.meeting && this.meeting.meetingTopic)
      this.meeting.meetingTopic = [];
  }

}
