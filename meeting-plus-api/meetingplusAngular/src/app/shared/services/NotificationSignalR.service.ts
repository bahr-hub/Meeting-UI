import { Injectable, InjectionToken, Optional, Inject, EventEmitter } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { environment } from '../../../environments/environment'
import { AuthService } from 'src/app/_Modules/auth/services/auth.service';
import { ToastrService } from 'ngx-toastr';


@Injectable({
  providedIn: 'root'
})
export class NotificationSignalRService {
  public NewNotification: any;
  private hubConnection: signalR.HubConnection;
  private baseUrl: string;
  receivedNotification: EventEmitter<any> = new EventEmitter<any>();

  constructor(public toasterService: ToastrService, private AuthService: AuthService) {
    this.baseUrl = environment.apiUrl ? environment.apiUrl : "http://localhost:6251/";
  }

  public startConnection = () => {
    const options: signalR.IHttpConnectionOptions = {
      accessTokenFactory: () => {
        return this.AuthService.currentUserValue.token;
      }
    };
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.baseUrl + 'notify', options)
      .build();
    this.hubConnection.onclose(async () => {
      await this.start();
    });
    this.hubConnection
      .start()
      .then(() => console.log('Connection started on notification signalr'))
      .catch(err => console.log('Error while starting connection 111: ' + err));
  }


  async start() {
    try {
      await this.hubConnection.start();

    } catch (err) {

      setTimeout(() => this.start(), 5000);
    }
  };



  public addBroadcastNotificationsListener = () => {
    this.hubConnection.on('updatenotification', (data) => {
      debugger;
      this.NewNotification = data;
      this.toasterService.warning(this.NewNotification.message);
      this.receivedNotification.emit(this.NewNotification);
      // var element = document.getElementById('notification-data');
      // var li = document.createElement("li");
      // li.innerHTML = ('<a ><i></i>' + this.NewNotification.message + '</a>');
      // element.insertBefore(li, element.firstChild);


    })
  }

}
