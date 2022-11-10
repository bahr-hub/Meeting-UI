import { NgModule } from '@angular/core';
import { SharedModule } from '../../../shared/shared.module';
import { LocationListComponent } from './components/location-list/location-list.component';
import { LocationRoutingModule } from './location-routing.module';
import { LocationService } from './services/location.service';
import { LocationFormComponent } from './components/location-form/location-form.component';
import { ItzButtonsModule } from 'src/app/shared/itz-buttons/itz-buttons.module';

@NgModule({
  imports: [
    LocationRoutingModule,
    SharedModule.forRoot(),
    ItzButtonsModule
  ],
  declarations: [
    LocationListComponent,
    LocationFormComponent
  ],
  providers: [
    LocationService
  ]
})

export class LocationModule {
}

