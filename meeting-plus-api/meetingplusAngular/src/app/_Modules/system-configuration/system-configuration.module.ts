import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { SystemConfigurationRoutingModule } from './system-configuration-routing.module';
import { SystemConfigurationService } from './services/system-configuration.service';
import { SystemConfigurationFormComponent } from './components/system-configuration-form/system-configuration-form.component';
import { CountryService } from 'src/app/_Modules/user/services/country.service';
import { CommonModule } from '@angular/common';

@NgModule({
  imports: [
    SystemConfigurationRoutingModule,
    CommonModule,
    SharedModule.forRoot()
  ],
  declarations: [
    SystemConfigurationFormComponent
  ],
  providers: [
    SystemConfigurationService,
    CountryService
  ]
})

export class SystemConfigurationModule {
}
