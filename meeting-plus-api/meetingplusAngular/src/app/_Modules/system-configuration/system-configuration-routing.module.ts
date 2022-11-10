import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SystemConfigurationFormComponent } from './components/system-configuration-form/system-configuration-form.component';

const routes: Routes = [
  { path: '', component: SystemConfigurationFormComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SystemConfigurationRoutingModule {
}
