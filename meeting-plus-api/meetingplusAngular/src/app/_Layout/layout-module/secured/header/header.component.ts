import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'src/app/_Modules/auth/services/auth.service';
import { Router, ActivatedRoute } from '@angular/router';
import { NotificationService } from 'src/app/shared/services/notification.service';
import { NotificationSignalRService } from 'src/app/shared/services/NotificationSignalR.service';
import { DataSource } from 'src/app/shared/models/data-source.model';
import { NotificationModel } from 'src/app/shared/models/notification.model';
import { MeetingLiteListComponent } from 'src/app/_Modules/meeting/home/components/meeting-lite-list/meeting-lite-list.component';
import { MeetingLiteListService } from 'src/app/_Modules/meeting/home/services/meeting-lite-list.service';

import { AbilityService } from 'src/app/shared/services/ability.service';
import { ThrowStmt } from '@angular/compiler';
@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {

  toggled: boolean;
  userName: string;
  public lang: string;
  toggleNotifications: boolean;
  notificationList: Array<NotificationModel>;
  selectedLang;
  dataSource: DataSource = new DataSource();
  searchText: string;

  constructor(public translate: TranslateService, private authService: AuthService, private router: Router,
    private notificationService: NotificationService, private notificationSignalRService: NotificationSignalRService,
    private meetingLiteListService: MeetingLiteListService
    , public ability: AbilityService
  ) {
    if( this.authService.currentUserValue){
      this.userName = this.authService.currentUserValue.name;
    }
    this.toggled = false;
    this.toggleNotifications = false;
    translate.addLangs(['en', 'ar']);
    this.setLanguage();
  }

  ngOnInit() {

    this.notificationSignalRService.startConnection();
    this.notificationSignalRService.addBroadcastNotificationsListener();

    //Notification List will listen if there is a notification receieved
    this.notificationSignalRService.receivedNotification.subscribe((notfication: any) => {
      console.log("Notification From NotificationService:" + notfication)
      if (!this.notificationList) this.notificationList = new Array<NotificationModel>();
      this.notificationList.push(notfication);
    });
    this.getAllNotifications();
  }

  private getAllNotifications() {
    this.dataSource.pageSize = 1000;
    this.notificationService.getAll(this.dataSource).subscribe(response => {
      this.notificationList = response.data;
    }, err => {
    });
  }
  public switchLang(lang: string) {
    this.translate.use(lang);
    localStorage.removeItem("lang");
    localStorage.setItem("lang", lang);
    let doc = document.getElementsByTagName("html")[0];
    if (lang == 'ar') {
      doc.dir = "rtl";
      doc.lang = "ar";
      
    }
    else {
      doc.dir = "ltr";
      doc.lang = "en";
   

    }

  }

  public openUserProfilePage() {
    this.hideOptionsMenu();
    let loggedUser = this.authService.currentUserValue;
    this.router.navigate(['users/user-form', loggedUser.id, true])
  }

  public logout() {
    this.hideOptionsMenu();
    this.authService.logout();
    this.router.navigate(["/Auth/Login"]);
  }

  private setLanguage() {
    this.lang = localStorage.getItem("lang");
    this.translate.setDefaultLang(this.lang);

  }

  public search() {
    this.router.navigate(["/Meetings/meeting-lite-list", this.searchText]);
    this.meetingLiteListService.search(this.searchText);
  }
  
  public hideOptionsMenu()
  {
    this.toggled = !this.toggled;
  }

}
