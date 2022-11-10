import { Component, OnInit, Input } from '@angular/core';
import { HomeService } from '../../services/home.service';
import { ToastrService } from 'ngx-toastr';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-todo-item',
  templateUrl: './todo-item.component.html',
  styleUrls: ['./todo-item.component.scss']
})

export class TodoItemComponent implements OnInit {
  @Input() meeting: any;
  opened: boolean;
  constructor(private homeService: HomeService, private toaster: ToastrService, private translate: TranslateService) {
    this.opened = false;
  }

  ngOnInit() {
    
  }


  closeTask(task: any) {
  
    this.homeService.closeTask(task.id)
      .subscribe((response) => {
        if (response.success) {
          this.meeting.filter(x => x.meetingTask.find(x => x.id === task.id).status = response.data.status);
          this.toaster.success(this.translate.instant(response.message))
        }
        else {
          task.status = false;
          this.toaster.error(this.translate.instant(response.message))
        }
      })
  }

  toggleTasks() {
    this.opened = !this.opened;
    
  }
}
