import { Component, OnInit, OnChanges } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import {
  FormGroup,
  FormBuilder,
  Validators,
  FormArray,
  AbstractControl
} from "@angular/forms";
import { MeetingOperationsService } from "../../meeting-operation/services/meeting-opertaions.service";
import { TagService } from "src/app/_Modules/lockups/tag/services/tag.service";
import {
  dateGreaterThan,
  dateSmallerThan
} from "../../../../shared/validators/date.validator";
import { DataSource } from "src/app/shared/models/data-source.model";
import { LocationService } from "src/app/_Modules/lockups/location/services/location.service";
import { ProjectService } from "src/app/_Modules/lockups/project/services/project.service";
import { AuthService } from 'src/app/_Modules/auth/services/auth.service';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: "app-meeting",
  templateUrl: "./meeting.component.html",
  styleUrls: ["./meeting.component.scss"]
})
export class MeetingComponent implements OnInit {
  meetingForm: FormGroup;
  availableParticipants: Array<any>;
  selectedParticipants:any;
  meetingTags: any;
  meetingLocations:  any;
  meetingProjects: any;

  dropdownSettingsParticipants:IDropdownSettings;

  constructor (
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private meetingService: MeetingOperationsService,
    private tagService: TagService,
    private locationService: LocationService,
    private projectService: ProjectService,
    private authService: AuthService,
    private toastService: ToastrService,
    private router:Router
  ) {
    this.loadData();
    const meetingId = this.route.snapshot.paramMap.get("id"); // get the meeting id
    meetingId ? this.loadMeeting(meetingId) : this.prepareForm();
    this.availableParticipants = new Array<any>();
    this.selectedParticipants = [];
    this.dropDownSettingSetter();
  }

  private dropDownSettingSetter()
  {
    this.dropdownSettingsParticipants={
      singleSelection: false,
      idField: 'id',
      textField: 'name',
      selectAllText: 'Select All',
      unSelectAllText: 'UnSelect All',
      itemsShowLimit: 3,
      allowSearchFilter: true
    }
  }

  ngOnInit() {
    //this.onChanges();
  }

  private loadData() {
    this.loadTags();
    this.loadProjects();
  }

  private loadMeeting(meetingId) {
    //TODO : load the meeting data from the services here
    this.meetingService.get(meetingId)
    .subscribe(res =>
      {
        this.prepareForm(res.data)
        this.loadTimeBasedData();
      });
  }

  private loadParticipants() {
    let startDate = this.meetingForm.get("from").value;
    let endDate = this.meetingForm.get("to").value;
    if (!(startDate && endDate))
    {
      return;
    }
    this.meetingService
      .getParticipants(new Date(startDate).toISOString(),new Date(endDate).toISOString())
      .subscribe(res => {
        let data = [];
        res.data.forEach(participant => {
          delete Object.assign(participant, {["participantId"]: participant["id"] })["id"];
          const {participantId , name} = participant;
          data.push({participantId,name});
        });
        this.availableParticipants = data;
        const {id, name} = this.authService.currentUserValue;
        const currentParticipant = {"participatnId":id,"name":name};
        if (!this.meetingForm.get("meetingParticipant").value)
        {
          this.meetingForm.get("meetingParticipant").setValue([currentParticipant]);
        }
        
      });
  }

  private loadTags() {
    let dataSoucre: DataSource = new DataSource();
    dataSoucre.pageSize = 100000;
    this.tagService.getAll(dataSoucre).subscribe(res => {
      let data = [];
      res.data.forEach(tag => {
        const {id , name} = tag;
        data.push({"tagId":id,"name":name});
      });
      
      this.meetingTags = data;
    });
  }

  private loadLocations() {
    let startDate = this.meetingForm.get("from").value;
    let endDate = this.meetingForm.get("to").value;
    if (!(startDate && endDate))
    {
      return;
    }
    this.meetingService
      .getLocations(new Date(startDate).toISOString(),new Date(endDate).toISOString())
      .subscribe(res => {
        this.meetingLocations = res.data;
      });

      
  }

  private loadProjects() {
    let dataSoucre: DataSource = new DataSource();
    dataSoucre.pageSize = 100000;
    this.projectService.getAll(dataSoucre).subscribe(res => {
      this.meetingProjects = res.data;
    });
  }

  private loadTimeBasedData()
  {
    this.loadLocations();
    this.loadParticipants();
  }
  /**
   * preparing the form that would hold the data of the meeting
   * and adding the required validators to the form control
   */
  private prepareForm(meetingData?:any) {
    this.meetingForm = this.fb.group({
      name: [meetingData ? meetingData.name : "", [Validators.required, Validators.maxLength(50)]],
      from: [meetingData ? meetingData.from : null, [Validators.required]],
      to: [meetingData ? meetingData.to : null, [Validators.required]],
      projectId: [meetingData ? meetingData.projectId: null, [Validators.required]],
      locationId: [meetingData ? meetingData.locationId: null, [Validators.required]],
      meetingTag: [meetingData ? meetingData.meetingTag: "", [Validators.required]],
      meetingParticipant: [meetingData ? meetingData.meetingParticipant: null, [Validators.required]],
      followUpMeeting: [meetingData ? meetingData.followUpMeeting: null, [Validators.required]],
      meetingTopic: this.fb.array([]),
      status: [meetingData ? meetingData.status : 0]
    });
    this.meetingForm
      .get("to")
      .setValidators([
        Validators.required,
        (control: AbstractControl) =>
          dateGreaterThan(this.meetingForm.get("from").value)(control)
      ]);
    this.meetingForm
      .get("from")
      .setValidators([
        Validators.required,
        (control: AbstractControl) =>
          dateSmallerThan(this.meetingForm.get("to").value)(control)
      ]);
    //this.createTopicItem();
    this.onChanges();
  }

  public createTopicItem() {
    const topicItem = this.fb.group({
      name: ["", [Validators.required, Validators.maxLength(50)]],
      presenterId: [null, [Validators.required]],
      presenterName: [null, [Validators.required]],
      duration: ["", [Validators.required]] //todo create a custom validator for the sum of the time
    });
    this.topicsArray.push(topicItem);
    
  }

  get topicsArray() {
    return this.meetingForm.get("meetingTopic") as FormArray;
  }

  createMeeting() {
    // save the data

   
    //this.topicsArray.removeAt(this.topicsArray.length - 1);
    this.meetingService.create(this.meetingForm.value).subscribe(res =>
      {
        this.toastService.success("Meeting succefully created","Meeting Created");
        this.router.navigateByUrl("/Meetings");
        
      },error=>{
        this.toastService.error("Meeting couldn't be created","Meeting creation failed")
      });
  }

  removeTopic(index) {
    if (this.topicsArray.length > 1) this.topicsArray.removeAt(index);
  }
  setPresenterName(topic)
  {}

  //checking for the changing of the app:
  onChanges() {
    this.meetingForm.get("from").valueChanges.subscribe(val => {
      this.loadTimeBasedData();
    });

    this.meetingForm.get("to").valueChanges.subscribe(val => {
      this.loadTimeBasedData();
    });
  }
}
