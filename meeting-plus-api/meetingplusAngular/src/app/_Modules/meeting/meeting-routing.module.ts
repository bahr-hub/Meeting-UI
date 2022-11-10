import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';


const routes: Routes = [
  {path:'',loadChildren: ()=> import("../meeting/home/home.module").then(mod => mod.HomeModule)},
  {path:'',loadChildren: ()=> import("../meeting/view/view.module").then(mod => mod.ViewModule)},
  {path:'',loadChildren: ()=> import("../meeting/meeting-operation/meeting-operation.module").then(mod => mod.MeetingOperationModule)}

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MeetingRoutingModule { }
