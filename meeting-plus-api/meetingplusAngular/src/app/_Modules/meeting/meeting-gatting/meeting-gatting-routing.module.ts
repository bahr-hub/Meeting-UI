import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MeetingGattingComponent } from './gatting-chart/meeting-gatting.component';


const routes: Routes = [
  { path: '', component: MeetingGattingComponent },

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MeetingGattingRoutingModule { }
