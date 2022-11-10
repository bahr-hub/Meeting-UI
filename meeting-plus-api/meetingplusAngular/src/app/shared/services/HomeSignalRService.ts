import { Injectable, Optional, Inject, InjectionToken } from '@angular/core';
import * as signalR from '@aspnet/signalr';

import { environment } from '../../../environments/environment'
import { AuthService } from 'src/app/_Modules/auth/services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class HomeSignalRService {
  private hubConnection: signalR.HubConnection;
  private baseUrl: string;
  constructor(private AuthService: AuthService) {
    this.baseUrl = environment.apiUrl ? environment.apiUrl : "http://localhost:6251/";
  }

  public startConnection = () => {
    const options: signalR.IHttpConnectionOptions = {
      accessTokenFactory: () => {
        return this.AuthService.currentUserValue.token;
      }
    };
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.baseUrl + 'HomeHub', options)
      .build();
    this.hubConnection
      .start()
      .then(() => console.log('Connection started'))
      .catch(err => console.log('Error while starting connection 111: ' + err));
  };
  
}
