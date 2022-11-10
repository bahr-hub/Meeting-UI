import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { MeetingLiteListComponent } from './components/meeting-lite-list/meeting-lite-list.component';


const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'meeting-lite-list/:meetingSearch', component: MeetingLiteListComponent }
];


@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HomeRoutingModule { }
