import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ExternalLinkComponent } from './components/external-link/external-link.component';
import { MeetingFunctionsComponent } from './components/meeting-functions/meeting-functions.component';
import { MeetingOperationsService } from '../meeting-operation/services/meeting-opertaions.service';
import { SharedModule } from '../../../shared/shared.module';
import { PurposeTimeComponent } from './components/purpose-time/purpose-time.component';
import { ReactiveFormsModule } from '@angular/forms';
import { OwlNativeDateTimeModule, OwlDateTimeModule } from 'ng-pick-datetime';


@NgModule({

  imports: [
    CommonModule,
    SharedModule,
    ReactiveFormsModule,
    OwlNativeDateTimeModule,
    OwlDateTimeModule
  ],
  exports: [
    ExternalLinkComponent,
    MeetingFunctionsComponent
  ],
  declarations: [
    ExternalLinkComponent,
    MeetingFunctionsComponent,
    PurposeTimeComponent
  ],
  entryComponents: [
    ExternalLinkComponent
  ],
  providers: [
    MeetingOperationsService,
  ]
})
export class MeetingSharedModule { }
