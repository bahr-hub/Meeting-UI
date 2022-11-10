import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

import { ViewRoutingModule } from "./view-routing.module";
import { ViewComponent } from "./view/view.component";
import { MeetingHeadComponent } from "./meeting-head/meeting-head.component";
import { TimerModule } from "../../timer/timer.module";
import { AgendaListComponent } from "./agenda-list/agenda-list.component";
import { AgendaItemComponent } from "./agenda-item/agenda-item.component";
import { ReactiveFormsModule, FormsModule } from "@angular/forms";
import { ParticipantListComponent } from "./participant-list/participant-list.component";
import { ParticipantItemComponent } from "./participant-item/participant-item.component";
import { ActionItemsListComponent } from "./action-items-list/action-items-list.component";
import { ActionItemsComponent } from "./action-items/action-items.component";
import { OwlDateTimeModule, OwlNativeDateTimeModule } from "ng-pick-datetime";
import { QuillModule } from "ngx-quill";
import { ViewMeetingService } from "./services/view-meeting.service";
import { MeetingTopicService } from "./services/meeting-topic.service";
import { MeetingTaskService } from "./services/meeting-task.service";
import { MeetingSharedModule } from '../meeting-shared/meeting-shared.module';
import { ItzButtonsModule} from '../../../shared/itz-buttons/itz-buttons.module'; 
import { NgSelectModule } from '@ng-select/ng-select';
import { SharedModule } from 'src/app/shared/shared.module';

@NgModule({
  declarations: [
    ViewComponent,
    MeetingHeadComponent,
    AgendaListComponent,
    AgendaItemComponent,
    ParticipantListComponent,
    ParticipantItemComponent,
    ActionItemsListComponent,
    ActionItemsComponent 
  ],
  exports:[
    AgendaListComponent,
    AgendaItemComponent
  ],
  imports: [
    CommonModule,
    ViewRoutingModule,
    TimerModule,
    ReactiveFormsModule,
    OwlDateTimeModule,
    OwlNativeDateTimeModule,
    QuillModule.forRoot(),
    FormsModule,
    MeetingSharedModule,
    NgSelectModule,
    SharedModule,
    ItzButtonsModule
  ],
  providers: [ViewMeetingService,MeetingTopicService,MeetingTaskService]
})
export class ViewModule {}
