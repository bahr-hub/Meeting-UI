import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from 'src/app/_Modules/auth/components/login/login.component';
import { RedirectComponent } from 'src/app/redirect/redirect.component';
import { ViewComponent } from 'src/app/_Modules/meeting/view/view/view.component';


const routes: Routes = [
  {path:'',component: LoginComponent,pathMatch:'full'},
  { path: 'Redirect', component: RedirectComponent},
  {path: '' , loadChildren: ()=> import('./public/public.module').then(m => m.PublicModule)},
  {path: '' , loadChildren: ()=> import('./secured/secured.module').then(m => m.SecuredModule)},
  // {path:"anonymous/Meetings/view/:id/:access_token",component:ViewComponent},
  // {path:"anonymous/Meetings/view/:id",component:ViewComponent},
  

  {path:'**',component:LoginComponent}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LayoutModuleRoutingModule { }
