import { NgModule } from '@angular/core';
import { DashboardRoutingModule } from './dashboard-routing.module';
import { DashboardComponent } from './components/dashboard.component';
import { SharedModule } from '../../shared/shared.module';
import { DashboardService } from 'src/app/_Modules/dashboard/services/dashboard.service';

@NgModule({
  imports: [
    DashboardRoutingModule,
    SharedModule.forRoot()
  ],
  declarations: [
    DashboardComponent
  ],
  providers: [
    DashboardService
  ]
})

export class DashboardModule {
}
