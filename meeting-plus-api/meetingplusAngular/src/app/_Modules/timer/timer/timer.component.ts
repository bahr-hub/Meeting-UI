import { Component, Input, OnInit, OnDestroy, ElementRef, Output, EventEmitter } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import * as moment from 'moment';
import { AuthService } from 'src/app/_Modules/auth/services/auth.service';

@Component({
  selector: 'app-timer',
  templateUrl: './timer.component.html',
  styleUrls: ['./timer.component.scss']
})
export class TimerComponent {
  start: any;
  @Input()
  set Start(start) {
    this.start = start;
  }
  end: any;
  @Input()
  set End(end) {
    this.end = end;

  }
  @Output() zeroTrigger;
  timer: any;
  currentOffset;
  public displayTime: any = [];
  constructor(
    private el: ElementRef,
    private translate: TranslateService,
    private auth: AuthService
  ) {
    this.zeroTrigger = new EventEmitter(true);
    let currentUser = JSON.parse(this.auth.getCurrentUser());
    this.currentOffset = currentUser.fkUserConfiguration.timeZone.split(',')[1].replace("UTC", "");
  }

  ngOnInit(): void {
    this.timer = setInterval((A) => {

      if (this.start) {
        let dateUtced: string = this.start + this.currentOffset;
        this.displayTime = this.getTimeDiff(dateUtced, true);
      } else {

        let dateUtced: string = this.end + this.currentOffset;

        //this.end  = this.end + this.currentOffset;

        this.displayTime = this.getTimeDiff(dateUtced);
      }
    }, 1000);
  }

  ngOnDestroy() {
    this.stopTimer();
  }

  private getTimeDiff(datetime, useAsTimer = false) {

    datetime = new Date(datetime).getTime();
    var now = new Date().getTime();

    if (isNaN(datetime)) {
      return "";
    }

    var milisec_diff = datetime - now;
    if (useAsTimer) {
      milisec_diff = now - datetime;
    }

    // Zero Time Trigger
    if (milisec_diff <= 0) {
      this.zeroTrigger.emit("reached zero");
      return "00:00:00:00";
    }

    var days = Math.floor(milisec_diff / 1000 / 60 / (60 * 24));
    var date_diff = new Date(milisec_diff);
    var day_string = (days) ? this.twoDigit(days) + "" : "";
    var day_hours = days * 24;



    // Date() takes a UTC timestamp – getHours() gets hours in local time not in UTC. therefore we have to use getUTCHours()
    return [day_string, this.twoDigit(date_diff.getUTCHours())
      , this.twoDigit(date_diff.getMinutes())
      , this.twoDigit(date_diff.getSeconds())];

  }


  private twoDigit(number: number) {
    return number > 9 ? "" + number : "0" + number;
  }

  private stopTimer() {
    clearInterval(this.timer);
    this.timer = undefined;
  }
}
