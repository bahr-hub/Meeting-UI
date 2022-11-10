import { Component, ViewEncapsulation, ViewChild, Input, OnInit } from '@angular/core';
import { DataSource } from '../../../../../shared/models/data-source.model';
import { AlertService } from 'src/app/shared/services/alert.service';
import { AbilityService } from 'src/app/shared/services/ability.service';
import { MeetingOperationsService } from 'src/app/_Modules/meeting/meeting-operation/services/meeting-opertaions.service';
import { ActivatedRoute } from '@angular/router';
import { MeetingLiteListService } from 'src/app/_Modules/meeting/home/services/meeting-lite-list.service';
import { ConfirmationModalComponent } from 'src/app/shared/components/confirmation-dialog/confirmation-modal.component';
import { ToastrService } from 'ngx-toastr';
import { TranslateService } from '@ngx-translate/core';
import { MeetingModel } from 'src/app/_Modules/meeting/meeting-shared/models/meeting.model';
import { AuthService } from 'src/app/_Modules/auth/services/auth.service';

@Component({
  selector: 'app-meeting-lite-list',
  templateUrl: './meeting-lite-list.component.html',
  styleUrls: ['./meeting-lite-list.component.scss']
})

export class MeetingLiteListComponent implements OnInit {
  meetingLiteList = [];
  dataSource: DataSource = new DataSource;
  public meetingSearch: string;
  total: number;
  @ViewChild(ConfirmationModalComponent) confirmationModalComponent;//This open dialog for confirmation delete

  constructor(private meetingOperationsService: MeetingOperationsService,
    private alertService: AlertService,
    public ability: AbilityService,
    private activatedRoute: ActivatedRoute,
    private meetingLiteListService: MeetingLiteListService,
    private toastr: ToastrService,
    private translate: TranslateService,
    private authService: AuthService
  ) {

  }

  ngOnInit() {
    this.meetingSearch = this.activatedRoute.snapshot.params["meetingSearch"];
    this.meetingLiteListService.searchMeetingName.subscribe((name: string) => {
      this.meetingSearch = name;
      this.getMeetingLiteList();
    });

  }

  openDialog(id: number): void {
    this.confirmationModalComponent.open(id);//Open dialog when you press in delete button
  }

  public delete(id) {
    this.meetingOperationsService.delete(id).subscribe(response => {
      this.handelRequestMessage(response)
    }, err => {
    });
  }

  public getMeetingLiteList() {

    let filter = {
      key: "Name",
      value: this.meetingSearch
    };
    this.dataSource.filter = [];
    this.dataSource.filter.push(filter);

    this.meetingOperationsService.getFiltredMeeting(this.dataSource).subscribe(response => {
      this.meetingLiteList = response.data;
      this.total = response.count;
    }, err => {
    });
  }
  public onChangePagination(data) {
    this.dataSource.pageSize = data.recordPerPage;
    this.dataSource.page = data.currentPage;
    this.getMeetingLiteList();
  }

  private handelRequestMessage(response) {
    if (response.success) {
      this.toastr.success(this.translate.instant(response.message));
    }
    else {
      this.toastr.error(this.translate.instant(response.message));
    }
  }

  public showDelete(meeting: MeetingModel) {
    //Sample
    ///if meeting is upcoming,then is not started ,
    // current but not started 
    //or previous but didn't start
    if (!meeting.startedAt && meeting.createdBy == JSON.parse(this.authService.getCurrentUser()).id) {
      return true;
    }
    return false;
  }
}
