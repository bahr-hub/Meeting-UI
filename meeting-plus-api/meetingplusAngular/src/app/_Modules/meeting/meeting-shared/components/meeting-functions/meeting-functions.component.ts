import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { Response } from '../../../../../shared/models/response.model';
import { ToastrService } from 'ngx-toastr';
import { MeetingOperationsService } from '../../../../../_Modules/meeting/meeting-operation/services/meeting-opertaions.service';
import { ConfirmationModalComponent } from '../../../../../shared/components/confirmation-dialog/confirmation-modal.component';
import { ExternalLinkComponent } from '../external-link/external-link.component';
import { Router } from '@angular/router';
import { AuthService } from '../../../../auth/services/auth.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-meeting-functions',
  templateUrl: './meeting-functions.component.html',
  styleUrls: ['./meeting-functions.component.scss']
})

export class MeetingFunctionsComponent implements OnInit {

  private domElem;
  isFullScreen: boolean;
  @Input() isViewPage: boolean = true;
  openPerposal: boolean;

  constructor(private toastr: ToastrService,
    private meetingOperationsService: MeetingOperationsService,
    private router: Router, private authService: AuthService,
    private translate: TranslateService) {
    this.isFullScreen = false;
    this.openPerposal = false;
  }
  @Input('meeting') meeting: any;
  @Input() meetingType: number;
  @ViewChild(ConfirmationModalComponent) confirmationModalComponent;//This open dialog for confirmation delete
  @ViewChild(ExternalLinkComponent) externalLinkComponent;//This open dialog for external link 

  ngOnInit() {

  }

  //#region  Meeting Functions
  public start(id) {
    this.meetingOperationsService.start(id).subscribe(response => {

      this.handelRequestMessage(response);
      if (response.success == true) {
        this.meeting.started = true;
        this.meeting.startedAt = response.data.startedAt;
        this.join(id);
      }
    }, err => {
    });
  }

  public end(id) {
    this.meetingOperationsService.end(id).subscribe(response => {
      this.handelRequestMessage(response)
    }, err => {
    });
  }

  public accept(id) {

    this.meetingOperationsService.accept(id).subscribe(response => {

      this.meeting.meetingParticipant.find(x => x.participantId == JSON.parse(this.authService.getCurrentUser()).id).response = 1;
      this.handelRequestMessage(response)
    }, err => {
    });
  }

  public decline(id) {
    this.meetingOperationsService.decline(id).subscribe(response => {
      this.meeting.meetingParticipant.find(x => x.participantId == JSON.parse(this.authService.getCurrentUser()).id).response = 0;
      this.handelRequestMessage(response)
    }, err => {
    });
  }

  public postpone(id) {
    this.openPerposal = !this.openPerposal
    // let date = new Date();//Send selected date
    // this.meetingOperationsService.propose(id, date).subscribe(response => {
    //   this.handelRequestMessage(response)
    // }, err => {
    // });
  }

  public delete(id) {
    this.meetingOperationsService.delete(id).subscribe(response => {
      this.handelRequestMessage(response)
    }, err => {
    });
  }

  public join(id) {
    this.meetingOperationsService.join(id).subscribe(response => {
      this.handelRequestMessage(response);

    }, err => {
      // this.toastr.error(this.translate.instant('locations.error'), this.translate.instant('locations.' + res.err_msg));
    });
  }

  public externalLink(id) {
    this.externalLinkComponent.open(id);
  }

  fullscreen() {

    this.isFullScreen = !this.isFullScreen;
    if (this.isFullScreen) {
      let domElem = document.documentElement as HTMLElement & {
        mozRequestFullScreen(): Promise<void>;
        webkitRequestFullscreen(): Promise<void>;
        msRequestFullscreen(): Promise<void>;
      };
      if (domElem.requestFullscreen) {
        domElem.requestFullscreen();
      } else if (domElem.mozRequestFullScreen) {
        /* Firefox */
        domElem.mozRequestFullScreen();
      } else if (domElem.webkitRequestFullscreen) {
        /* Chrome, Safari and Opera */
        domElem.webkitRequestFullscreen();
      } else if (domElem.msRequestFullscreen) {
        /* IE/Edge */
        domElem.msRequestFullscreen();
      }
    }
    else {
      const docWithBrowsersExitFunctions = document as Document & {
        mozCancelFullScreen(): Promise<void>;
        webkitExitFullscreen(): Promise<void>;
        msExitFullscreen(): Promise<void>;
      };
      if (docWithBrowsersExitFunctions.exitFullscreen) {
        docWithBrowsersExitFunctions.exitFullscreen();
      } else if (docWithBrowsersExitFunctions.mozCancelFullScreen) {
        /* Firefox */
        docWithBrowsersExitFunctions.mozCancelFullScreen();
      } else if (docWithBrowsersExitFunctions.webkitExitFullscreen) {
        /* Chrome, Safari and Opera */
        docWithBrowsersExitFunctions.webkitExitFullscreen();
      } else if (docWithBrowsersExitFunctions.msExitFullscreen) {
        /* IE/Edge */
        docWithBrowsersExitFunctions.msExitFullscreen();
      }
    }


  }

  public edit(id) {
    this.router.navigate(["Meetings/edit/", id]);
  }
  //#endregion

  //#region Meeting Validation
  isAcceptMeeting(meeting: any) {

    var userInMeeting = meeting.meetingParticipant.find(x => x.participantId == JSON.parse(this.authService.getCurrentUser()).id);
    if (userInMeeting)
      return userInMeeting.response;
    else
      return false;

  }

  isJoined(meeting): boolean {
    return meeting.meetingParticipant.find(x => x.participantId == JSON.parse(this.authService.getCurrentUser()).id).joinedMeeting
  }

  isMeetingCreator(meeting: any) {

    if (meeting.createdBy == JSON.parse(this.authService.getCurrentUser()).id) {
      return true;
    }
    return false;
  }

  canDeleteMeeting(meeting: any) {
    return meeting.createdBy == JSON.parse(this.authService.getCurrentUser()).id && (!meeting.started || !meeting.notEnded);
  }

  //#endregion
  public gotToPreviousMeeting(PreMeetingId) {
    this.router.navigate(["Meetings/view/", PreMeetingId]);
  }
  //#region Meeting Deleting
  openDialog(id: number): void {
    this.confirmationModalComponent.open(id);//Open dialog when you press in delete button
  }

  private handelRequestMessage(response) {
    if (response.success) {
      this.toastr.success(this.translate.instant(response.message));
    }
    else {
      this.toastr.error(this.translate.instant(response.message));
    }
  }

}
