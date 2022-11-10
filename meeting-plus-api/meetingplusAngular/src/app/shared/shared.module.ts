import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule, registerLocaleData } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, HttpClient } from '@angular/common/http';
import arSaLocale from '@angular/common/locales/ar-SA';
import { DataListComponent } from './components/data-list/data-list.component';
import { HeaderPipePipe } from './pipes/header-pipe.pipe';
import { ModalBasicComponent } from './components/modal-basic/modal-basic.component';
import { ConfirmationModalComponent } from './components/confirmation-dialog/confirmation-modal.component';
import { PaginationComponent } from './components/pagination/pagination.component';
import { MaterialModule } from './modules/material.module';
import { TitleComponent } from './title/title.component';
import { ItzCheckboxComponent } from './itz-checkbox/itz-checkbox.component';
import { TranslateModule } from '@ngx-translate/core';
import { LoaderComponent } from './components/loader/loader.component';
import { AlertService } from './services/alert.service';
import { FileUploaderComponent } from './components/file-uploader/file-uploader.component';
import { PhoneNumber } from 'src/app/shared/directive/phone-number.directive';
import { LocalizedDatePipe } from './pipes/CustomDatePipe';


@NgModule({

  imports: [
    CommonModule,
    FormsModule,
    TranslateModule
  ],
  providers: [
    AlertService
  ],
  exports: [
    CommonModule,
    FormsModule,
    DataListComponent,
    ModalBasicComponent,
    ConfirmationModalComponent,
    PaginationComponent,
    TitleComponent,
    ItzCheckboxComponent,
    TranslateModule,
    LoaderComponent,
    FileUploaderComponent,
    PhoneNumber
  ],
  declarations: [
    DataListComponent,
    HeaderPipePipe,
    ModalBasicComponent,
    ConfirmationModalComponent,
    PaginationComponent,
    TitleComponent,
    ItzCheckboxComponent,
    LoaderComponent,
    FileUploaderComponent,
    PhoneNumber
  ],
  entryComponents: [
  ],
})
export class SharedModule {
  static forRoot(): ModuleWithProviders<SharedModule> {
    return {
      ngModule: SharedModule
    };
  }
}



