import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MeetingDetailsRoutingModule } from './meeting-details-routing.module';
import { MeetingComponent } from './meeting/meeting.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime';
import { NgSelectModule } from '@ng-select/ng-select';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';

@NgModule({
  declarations: [MeetingComponent],
  imports: [
    CommonModule,
    MeetingDetailsRoutingModule,
    OwlDateTimeModule,
    OwlNativeDateTimeModule, 
    ReactiveFormsModule,
    NgSelectModule,
    FormsModule,
    NgMultiSelectDropDownModule.forRoot()
  ]
})
export class MeetingDetailsModule { }
