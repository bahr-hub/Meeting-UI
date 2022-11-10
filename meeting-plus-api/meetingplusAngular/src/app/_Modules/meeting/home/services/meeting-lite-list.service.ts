import { Injectable, InjectionToken, Optional, Inject, EventEmitter } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { AuthService } from 'src/app/_Modules/auth/services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class MeetingLiteListService {
  searchMeetingName = new EventEmitter();    
  constructor() {
  }

  public search(meetingName) {
    this.searchMeetingName.emit(meetingName);
  }
}

