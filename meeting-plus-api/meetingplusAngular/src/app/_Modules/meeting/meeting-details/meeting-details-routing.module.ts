import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MeetingComponent } from './meeting/meeting.component';
import { NgSelectModule } from '@ng-select/ng-select';
import { FormsModule } from '@angular/forms';

const routes: Routes = [
  {path:"edit/:id",component: MeetingComponent},
  {path:"create",component: MeetingComponent},
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MeetingDetailsRoutingModule { }
