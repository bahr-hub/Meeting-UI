import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { LayoutModuleModule } from './_Layout/layout-module/layout-module.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { HttpClientModule, HTTP_INTERCEPTORS, HttpClient } from '@angular/common/http';
import { RoutingStateService } from './shared/services/routing-state.service';
import { HttpConfigInterceptor } from './shared/interceptors/httpconfig.interceptor';
import { AuthModule } from './_Modules/auth/auth.module';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { QuillModule } from 'ngx-quill';
import { CommonModule } from '@angular/common';

import { SharedModule } from 'src/app/shared/shared.module';
import { RedirectComponent } from './redirect/redirect.component';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { MeetingTaskService } from './_Modules/meeting/view/services/meeting-task.service';


@NgModule({
  declarations: [
    AppComponent,
    RedirectComponent
  ],
  imports: [
    CommonModule,
    AuthModule,
    RouterModule.forRoot([]),
    LayoutModuleModule,
    HttpClientModule,
    NgbModule,
    BrowserAnimationsModule, // required animations module
    ToastrModule.forRoot(), // ToastrModule added
    QuillModule.forRoot(),
    TranslateModule.forRoot({
      loader: {
          provide: TranslateLoader,
          useFactory: createTranslateLoader,
          deps: [HttpClient]
      }
  })
  ],
  providers: [
    ToastrService,
    MeetingTaskService,//Wrong location but for testing
    RoutingStateService,
    { provide: HTTP_INTERCEPTORS, useClass: HttpConfigInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }


export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http);
}
