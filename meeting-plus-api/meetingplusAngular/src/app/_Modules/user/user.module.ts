import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { UserFormComponent } from './components/user-form/user-form.component';
import { UserListComponent } from './components/user-list/user-list.component';
import { UserRoutingModule } from './user-routing.module';
import { UserService } from './services/user.service';
import { UserProfileComponent } from './components/user-profile/user-profile.component';
import { CountryService } from './services/country.service';
import { RoleService } from '../lockups/role/services/role.service';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NgSelectModule } from '@ng-select/ng-select';
import { ItzButtonsModule } from 'src/app/shared/itz-buttons/itz-buttons.module';
@NgModule({
  imports: [
    UserRoutingModule,
    SharedModule.forRoot(),
    ReactiveFormsModule,
    CommonModule,
    NgSelectModule,
    ItzButtonsModule
  ],
  declarations: [
    UserFormComponent,
    UserListComponent,
    UserProfileComponent
  ],
  providers: [
    UserService,
    CountryService,
    RoleService,
  ]
})

export class UserModule {
}

