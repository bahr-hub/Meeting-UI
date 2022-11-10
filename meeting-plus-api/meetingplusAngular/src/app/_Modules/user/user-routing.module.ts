import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UserFormComponent } from './components/user-form/user-form.component';
import { UserListComponent } from './components/user-list/user-list.component';
import { UserProfileComponent } from 'src/app/_Modules/user/components/user-profile/user-profile.component';

const routes: Routes = [
  { path: '', component: UserListComponent },
  { path: 'user-list', component: UserListComponent },
  { path: 'user-form', component: UserFormComponent },
  { path: 'user-form/:id', component: UserFormComponent },
  { path: 'user-form/:id', component: UserFormComponent },
  { path: 'user-form/:id/:fromHeader', component: UserFormComponent },

  { path: 'user-profile/:id', component: UserProfileComponent },

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UserRoutingModule {
}
