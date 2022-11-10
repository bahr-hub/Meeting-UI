import { NgModule } from '@angular/core';
import { SharedModule } from '../../../shared/shared.module';
import { RoleListComponent } from './components/role-list/role-list.component';
import { RoleRoutingModule } from './role-routing.module';
import { RoleService } from './services/role.service';
import { RoleFormComponent } from './components/role-form/role-form.component';
import { ItzButtonsModule } from 'src/app/shared/itz-buttons/itz-buttons.module';

@NgModule({
  imports: [
    RoleRoutingModule,
    ItzButtonsModule,
    SharedModule.forRoot(),
  ],
  declarations: [
    RoleListComponent,
    RoleFormComponent
  ],
  providers: [
    RoleService
  ],
  entryComponents: [
    RoleFormComponent
  ]
})

export class RoleModule {
}
