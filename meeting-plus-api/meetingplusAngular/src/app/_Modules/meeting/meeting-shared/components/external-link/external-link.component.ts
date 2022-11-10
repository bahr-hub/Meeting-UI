import { Component, ViewChild, EventEmitter, Output } from '@angular/core';
import { NgbModal, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';
import { MeetingOperationsService } from '../../../meeting-operation/services/meeting-opertaions.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-external-link',
  templateUrl: './external-link.component.html',
  styleUrls: ['./external-link.component.css']
})
export class ExternalLinkComponent {
  closeResult: string;
  @ViewChild('content') content: any;
  currentMeeting: any;
  id: number;
  constructor(private modalService: NgbModal, private meetingOperationsService: MeetingOperationsService, public translate: TranslateService) { }

  open(id: number) {
    this.meetingOperationsService.get(id).subscribe(response => {
      this.currentMeeting = response.data;
      this.modalService.open(this.content);
    }, err => {
    });
  }

}
