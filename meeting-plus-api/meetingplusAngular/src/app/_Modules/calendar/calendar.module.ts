import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { CalendarRoutingModule } from './calendar-routing.module';
import { CalendarComponent } from './components/calendar/calendar.component';
import { CalendarService } from './services/calendar.service';
import { FullCalendarModule } from '@fullcalendar/angular';


@NgModule({
  imports: [
    CalendarRoutingModule,
    SharedModule.forRoot(),
    FullCalendarModule  
  ],
  declarations: [
    CalendarComponent
  ],
  providers: [
    CalendarService
  ]
})

export class CalendarModule {
}
