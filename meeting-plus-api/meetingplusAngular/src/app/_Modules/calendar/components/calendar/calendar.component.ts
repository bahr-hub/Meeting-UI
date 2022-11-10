import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import arlocale from '@fullcalendar/core/locales/ar'
import enlocale from '@fullcalendar/core/locales/en-au'
import dayGridMonth from '@fullcalendar/daygrid';
import dayGridWeek from '@fullcalendar/daygrid';
import dayGridDay from '@fullcalendar/daygrid';

import { Router } from '@angular/router';
import { CalendarService } from '../../services/calendar.service';
import { ThrowStmt } from '@angular/compiler';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-calendar',
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.css']
})

export class CalendarComponent implements OnInit {

  calendarPlugins = [dayGridMonth, dayGridWeek, dayGridDay];
  calendarLocales = [enlocale, arlocale];
  meetings: Array<any>;
  loading: boolean;
  header = {
    left: 'prev,next today',
    center: 'title',
    right: 'dayGridMonth,dayGridWeek,dayGridDay,timeGridWeek,timeGridDay'
  }
  constructor(private calendarService: CalendarService,
    private router: Router,
    public translate: TranslateService) {
      this.meetings = [];
    this.loading = true;
     }

  ngOnInit() {
    let date = new Date();
    let start = new Date(date.getFullYear(), date.getMonth(), 1).toISOString();
    let end = new Date(date.getFullYear(), date.getMonth() + 1, 0).toISOString(); 
    this.loadCalender(start,end);
  }
  dateChanged(info)
  {
    let start = new Date(info.currentStart).toISOString();
    let end = new Date(info.currentEnd).toISOString();
    if(this.meetings.length === 0)
      this.loadCalender(start,end);
  }

  openMeeting(info)
  {
    this.router.navigateByUrl(`/Meetings/view/${info.event.id}`)
  }


  private loadCalender(start,end)
  {
    this.setLoading(true);
    this.calendarService.getInterval(start,end)
    .subscribe(res=>{
      this.meetings=[]
      res.data.forEach(meeting => {
        const {name,from,to,id} = meeting
        let event = {id: id, title: name, date: from, end: to , start: from }
        this.meetings.push(event);
      });
      this.setLoading(false);
    });
  }

  private setLoading(value: boolean)
  {
    setTimeout(() => {
      this.loading = value;
    }, 500);
  }
 

 

}
