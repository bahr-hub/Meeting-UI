
import { environment } from '../../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/internal/operators/catchError';
import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';

@Injectable()

export class AlertService {

    constructor(private translate: TranslateService, private toaster: ToastrService) {

    }

  public handleResponseMessage(response: any) {
    if (response.message) {
      if (response.success) {
        this.toaster.success(this.translate.instant(response.message));
      }
      else {
        this.toaster.error(this.translate.instant(response.message));
      }
    }
    return response.success;
    }
}
