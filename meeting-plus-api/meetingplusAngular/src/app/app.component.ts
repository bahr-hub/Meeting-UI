import { Component } from '@angular/core';
import { RoutingStateService } from './shared/services/routing-state.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'MeetingPlus';

  constructor(private routingStateService: RoutingStateService) {
    this.routingStateService.loadRouting();

  }

}
