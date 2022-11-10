import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { BaseComponent } from './base/base.component';
import { AuthGuardService } from 'src/app/shared/guards/auth-guard.service';


const routes: Routes = [
  {
    path: "",
    component: BaseComponent,
    children: [
      { path: 'Meetings', loadChildren: () => import('../../../_Modules/meeting/meeting.module').then(m => m.MeetingModule) },
      { path: 'locations', loadChildren: () => import('../../../_Modules/lockups/location/location.module').then(m => m.LocationModule) },
      { path: 'roles', loadChildren: () => import('../../../_Modules/lockups/role/role.module').then(m => m.RoleModule) },
      { path: 'tags', loadChildren: () => import('../../../_Modules/lockups/tag/tag.module').then(m => m.TagModule) },
      { path: 'projects', loadChildren: () => import('../../../_Modules/lockups/project/project.module').then(m => m.ProjectModule) },
      { path: 'calendar', loadChildren: () => import('../../../_Modules/calendar/calendar.module').then(m => m.CalendarModule) },
      { path: 'users', loadChildren: () => import('../../../_Modules/user/user.module').then(m => m.UserModule) },
      { path: 'system-configurations', loadChildren: () => import('../../../_Modules/system-configuration/system-configuration.module').then(m => m.SystemConfigurationModule) },
      { path: 'Redirect', loadChildren: () => import('src/app/redirect/redirect.component').then(m => m.RedirectComponent) },
      { path: 'Chart', loadChildren: () => import('../../../_Modules/meeting/meeting-gatting/meeting-gatting.module').then(m => m.MeetingGattingModule) },

    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SecuredRoutingModule { }
