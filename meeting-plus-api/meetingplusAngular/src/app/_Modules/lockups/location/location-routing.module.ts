import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LocationListComponent } from './components/location-list/location-list.component';

const routes: Routes = [
  { path: '', component: LocationListComponent },
  { path: 'location-list', component: LocationListComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LocationRoutingModule {
}
