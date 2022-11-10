import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GattingService } from './services/gatting.service'
import { MeetingGattingRoutingModule } from './meeting-gatting-routing.module';
import { MeetingGattingComponent } from './gatting-chart/meeting-gatting.component';
import { GanttModule, EditService, FilterService, SortService, SelectionService, ToolbarService, DayMarkersService } from '@syncfusion/ej2-angular-gantt';


@NgModule({
  declarations: [MeetingGattingComponent],
  imports: [
    CommonModule,
    MeetingGattingRoutingModule,
    GanttModule,

  ],
  providers: [GattingService, EditService, FilterService, SortService, SelectionService, ToolbarService, DayMarkersService]
})
export class MeetingGattingModule { }
