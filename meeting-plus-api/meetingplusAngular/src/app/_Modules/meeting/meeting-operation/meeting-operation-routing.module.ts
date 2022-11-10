import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { OperationComponent } from './operation/operation.component';


const routes: Routes = [
  { path: 'create', component: OperationComponent },
  { path: 'edit/:id', component: OperationComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MeetingOperationRoutingModule { }
