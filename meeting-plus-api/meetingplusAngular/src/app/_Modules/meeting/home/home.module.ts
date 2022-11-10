import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HomeRoutingModule } from './home-routing.module';
import { HomeComponent } from './components/home/home.component';
import { WorkingDaysComponent } from './components/working-days/working-days.component';
import { MeetingsListComponent } from './components/meetings-list/meetings-list.component';
import { MeetingCardComponent } from './components/meeting-card/meeting-card.component';
import { TimerModule } from '../../timer/timer.module';
import { TodoListComponent } from './components/todo-list/todo-list.component';
import { TodoItemComponent } from './components/todo-item/todo-item.component';
import { HomeService } from './services/home.service';
import { SystemConfigurationService } from '../../system-configuration/services/system-configuration.service';
import { MeetingOperationsService } from '../meeting-operation/services/meeting-opertaions.service';
import { SharedModule } from '../../../shared/shared.module';
import { MeetingSharedModule } from '../meeting-shared/meeting-shared.module';
import { MeetingLiteListService } from 'src/app/_Modules/meeting/home/services/meeting-lite-list.service';
import { MeetingLiteListComponent } from 'src/app/_Modules/meeting/home/components/meeting-lite-list/meeting-lite-list.component';
import { LocalizedDatePipe } from '../../../shared/pipes/CustomDatePipe';

@NgModule({

  imports: [
    CommonModule,
    HomeRoutingModule,
    TimerModule,
    SharedModule,
    MeetingSharedModule
  ],
  declarations: [
    HomeComponent,
    LocalizedDatePipe,
    WorkingDaysComponent,
    MeetingsListComponent,
    MeetingCardComponent,
    TodoListComponent,
    TodoItemComponent,
    MeetingLiteListComponent
  ],
  providers: [
    HomeService,
    MeetingOperationsService,
    SystemConfigurationService
  ],



})
export class HomeModule { }
