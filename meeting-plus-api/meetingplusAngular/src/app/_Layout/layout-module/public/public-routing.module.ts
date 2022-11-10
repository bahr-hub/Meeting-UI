import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ViewComponent } from 'src/app/_Modules/meeting/view/view/view.component';

const routes: Routes = [
  {path: 'Auth' , loadChildren: () => import('../../../_Modules/auth/auth.module').then(m => m.AuthModule)},
 
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PublicRoutingModule { }
