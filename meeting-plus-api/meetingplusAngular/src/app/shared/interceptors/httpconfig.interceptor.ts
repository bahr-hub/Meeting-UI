import { Injectable } from '@angular/core';
// import { ErrorDialogService } from '../error-dialog/errordialog.service';
import { HttpInterceptor, HttpRequest, HttpResponse, HttpHandler, HttpEvent, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { map, catchError, tap } from 'rxjs/operators';
import { AuthService } from '../../_Modules/auth/services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';

@Injectable()
export class HttpConfigInterceptor implements HttpInterceptor {

    constructor(private authService: AuthService, private toastr:ToastrService
      ,private router: Router ) {

  }
  getDefaultHeaders() {
    var currentUser = JSON.parse(this.authService.getCurrentUser());

    return {
      'Accept': 'application/json',
      'Content-Type': 'application/json',
      'Allow-Origin': '*',
      'Enabled': 'true',
      'Allow-Headers': 'Authorization, Content-Type,Allow-Origin',
      'Authorization': 'bearer ' + currentUser.token
    }
  };
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (request.url == "https://restcountries.eu/rest/v2/all") {//Countries
      return next.handle(request);
    }
    var currentUser = JSON.parse(this.authService.getCurrentUser());
    if (currentUser && currentUser.token) {
      //   request = request.clone({ headers: request.headers.set('Authorization', 'Bearer ' + currentUser.token) });
      request = request.clone({ headers: new HttpHeaders(this.getDefaultHeaders()) });
    }

    if (!request.headers.has('Content-Type')) {
      request = request.clone({ headers: request.headers.set('Content-Type', 'application/json') });
    }

    request = request.clone({ headers: request.headers.set('Accept', 'application/json') });
    return next.handle(request).pipe(
      //EMAN
      tap(evt => {
        if (evt instanceof HttpResponse) {
          // this.toastr.success(evt.body.statusCode,"LiscenseExpired");
          if (evt.body && evt.body.statusCode === 707)//LiscenseExpired
            this.toastr.error(evt.body.message, evt.body.statusCode);
        }
      }),
      catchError((error) => {
        if (error.status == 401) {
          this.toastr.error('ErrorMsg.unauthorize');
        } else if (error.status = 400) {
          this.toastr.error('ErrorMsg.badRequest');
        }
        return throwError(error);
      }))

      ;
  }
}