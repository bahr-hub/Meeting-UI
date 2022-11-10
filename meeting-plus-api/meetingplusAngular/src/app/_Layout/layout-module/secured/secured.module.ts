import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SecuredRoutingModule } from './secured-routing.module';
import { BaseComponent } from './base/base.component';
import { SideBarComponent } from './side-bar/side-bar.component';
import { SideBarItemComponent } from './side-bar-item/side-bar-item.component';
import { HeaderComponent } from './header/header.component';
import { AuthService } from '../../../_Modules/auth/services/auth.service';
import { HttpClient } from '@angular/common/http';
import { SharedModule } from 'src/app/shared/shared.module';
import { NotificationService } from "src/app/shared/services/notification.service";
import { MeetingLiteListService } from 'src/app/_Modules/meeting/home/services/meeting-lite-list.service';



@NgModule({
  declarations: [BaseComponent, SideBarComponent, SideBarItemComponent, HeaderComponent],
  imports: [
    CommonModule,
    SecuredRoutingModule,
    SharedModule.forRoot(),
  ],
  providers: [
    NotificationService
  ]
})
export class SecuredModule { }
