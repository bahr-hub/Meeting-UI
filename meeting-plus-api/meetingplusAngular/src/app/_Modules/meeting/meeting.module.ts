import { NgModule } from '@angular/core';
import { CommonModule, registerLocaleData } from '@angular/common';
import ar from '@angular/common/locales/ar'
registerLocaleData(ar);

import { MeetingRoutingModule } from './meeting-routing.module';
import { MeetingTaskService } from './view/services/meeting-task.service';
// import { MeetingGattingComponent } from './meeting-getting/gatting-chart/meeting-gatting.component';


@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    MeetingRoutingModule,

  ],

})
export class MeetingModule { }
