import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LayoutModuleRoutingModule } from './layout-module-routing.module';

import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ViewModule } from 'src/app/_Modules/meeting/view/view.module';


@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    LayoutModuleRoutingModule,
    ViewModule

  ],
  providers: [TranslateService
    // ViewMeetingService
  

  ]
})
export class LayoutModuleModule { }
