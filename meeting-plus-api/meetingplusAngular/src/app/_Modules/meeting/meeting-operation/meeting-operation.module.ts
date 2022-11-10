import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MeetingOperationRoutingModule } from './meeting-operation-routing.module';
import { OperationComponent } from './operation/operation.component';
import { ReactiveFormsModule } from '@angular/forms';
import { OwlNativeDateTimeModule, OwlDateTimeModule } from 'ng-pick-datetime';
import { NgSelectModule } from '@ng-select/ng-select';
import { SharedModule } from 'src/app/shared/shared.module';
import { ViewModule } from '../view/view.module';



@NgModule({
  declarations: [OperationComponent],
  imports: [
    CommonModule,
    MeetingOperationRoutingModule,
    ReactiveFormsModule,
    OwlDateTimeModule,
    OwlNativeDateTimeModule, 
    NgSelectModule,
    SharedModule,
    ViewModule
  ]
})
export class MeetingOperationModule { }
