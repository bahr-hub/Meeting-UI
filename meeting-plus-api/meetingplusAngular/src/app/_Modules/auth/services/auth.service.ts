import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { UserModel } from '../../../_Modules/user/models/user.model';
import { UserLoginModel } from '../../../_Modules/auth/models/user-login.model';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Router, RoutesRecognized } from '@angular/router';
import { filter } from 'rxjs/internal/operators/filter';
import { pairwise } from 'rxjs/internal/operators/pairwise';

@Injectable(
  { providedIn: 'root' }
)

export class AuthService {
  private baseUrl: string;

  constructor(private _http: HttpClient, private router: Router) {
    this.baseUrl = environment.apiUrl + "api/";
  }

  public get currentUserValue(): UserModel {
    return JSON.parse(this.getCurrentUser());
  }

  public login(user: UserLoginModel): any {
    debugger;
    user.userDate = new Date();
    return this._http.post(`${this.baseUrl}Account/Authenticate/`, user);

  }
  public guestLogin():any
  {
    var user = new UserLoginModel();
    user.email = 'mody199513@gmail.com';
    user.password ='12345678'
    user.userDate = new Date();
    return this._http.post(`${this.baseUrl}Account/Authenticate/`, user);
  }

  getReturnUrl() {
    return this.router.events
      .pipe(filter((evt: any) => evt instanceof RoutesRecognized), pairwise());
  }

  setCurrentUser(currentUser) {
    localStorage.setItem('currentUser', currentUser);
  }

  getCurrentUser(): any {
    return (localStorage.getItem('currentUser'));
  }

  setToken(token) {
    localStorage.setItem('token', token);
  }

  getToken(): any {
    return localStorage.getItem('token');
  }


  logout() {
    // remove user from local storage to log user out
    localStorage.removeItem('currentUser');
    // this.currentUserSubject.next(null);
    localStorage.removeItem('token');
  }

}
