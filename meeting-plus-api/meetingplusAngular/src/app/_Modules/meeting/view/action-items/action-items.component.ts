import { Component, OnInit, Input } from "@angular/core";
import * as moment from "moment";
import { ToastrService } from 'ngx-toastr';
import { MeetingTaskService } from "../services/meeting-task.service";

@Component({
  selector: "app-action-items",
  templateUrl: "./action-items.component.html",
  styleUrls: ["./action-items.component.scss"]
})
export class ActionItemsComponent implements OnInit {
  @Input("actionItem") actionItem;
  constructor( private toastr:ToastrService, private MeetingTaskService:MeetingTaskService) {}

  ngOnInit() {
    this.actionItem["dueDate"] = moment(this.actionItem["dueDate"]).format(
      "HH:MM DD-MM"
    );
  }

  closeTask(id:any)
  {
    this.MeetingTaskService.CloseTask(id).subscribe(response => {
      if (response.success) {
        this.toastr.success(response.message)
      }
      else {
        this.actionItem.status = false;
        this.toastr.error(response.message)
      }
    }, err => {
  });
  }
}
