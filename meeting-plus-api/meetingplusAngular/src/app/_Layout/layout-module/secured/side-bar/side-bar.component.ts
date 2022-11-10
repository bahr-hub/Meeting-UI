import { Component, OnInit, Output,EventEmitter} from '@angular/core';
import { AuthService } from '../../../../_Modules/auth/services/auth.service';
import { Router } from '@angular/router';
import { AbilityService } from '../../../../shared/services/ability.service';


@Component({
  selector: 'app-side-bar',
  templateUrl: './side-bar.component.html',
  styleUrls: ['./side-bar.component.scss']
})
export class SideBarComponent implements OnInit {

  @Output() expanded: EventEmitter<any> = new EventEmitter();
  isExpanded: boolean;
  Links: any;
  
  constructor(private authService: AuthService, private router: Router, public ability: AbilityService) {
    this.Links = [
      { url: "/Meetings/create", img: "../../../../../assets/images/add.png", action: "create", mod: "meeting", name:"sidebar.create_meeting" },
      { url: "/Meetings", img: "../../../../../assets/images/meeting.png", action: "view", mod: "meeting", name: "sidebar.home" },
      { url: "/locations", img: "../../../../../assets/images/maps-and-flags.png", action: "view", mod: "location", name:"sidebar.locations" },
      { url: "/users", img: "../../../../../assets/images/users-group.png", action: "view", mod: "user",name:"sidebar.users" },
      { url: "/tags", img: "../../../../../assets/images/tag.png", action: "view", mod: "tag", name:"sidebar.tags" },
      { url: "/projects", img: "../../../../../assets/images/bag.png", action: "view", mod: "project", name:"sidebar.projects" },
      { url: "/roles", img: "../../../../../assets/images/authorization.png", action: "view", mod: "role", name:"sidebar.roles" },
      { url: "/calendar", img: "../../../../../assets/images/sidemeu_cal.png", action: "view", mod: "calendar", name: "sidebar.calendar" },
      { url: "/Chart", img: "../../../../../assets/images/gantt-chart.png", action: "view", mod: "chart", name: "sidebar.chart" }
    ]
    
      this.isExpanded = false;
    }

  ngOnInit() {
  }

  public logout() {
    this.authService.logout();
    this.router.navigate(["/Auth/Login"]);
  }
  expand()
  {
    this.isExpanded = !this.isExpanded;
    this.expanded.emit();
  }
}
