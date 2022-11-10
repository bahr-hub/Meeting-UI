import { Component, OnInit } from '@angular/core';
import { DataSource } from 'src/app/shared/models/data-source.model';
import { GattingService } from '../services/gatting.service';
//import { editingData, editingResources } from '../data';

@Component({
  selector: 'app-meeting-getting',
  templateUrl: './meeting-gatting.component.html',
  styleUrls: ['./meeting-gatting.component.scss']
})



export class MeetingGattingComponent implements OnInit {

  constructor(private gattingService: GattingService) { };

  public data: object[] = [];
  public resources: object[];
  public resourceFields: object;
  public taskSettings: object;
  public columns: object[];
  public timelineSettings: object;
  public gridLines: string;
  public labelSettings: object;
  public projectStartDate: Date;
  public projectEndDate: Date;
  public editSettings: object;
  public eventMarkers: object[];
  public toolbar: string[];
  public splitterSettings: object;
  public dayWorkingTime: object[];
  public workWeek: string[];
  

  dataSource: DataSource = new DataSource();

  private getAllMeetings() {
    var filter = {
      key: "From",
      value: new Date().toLocaleDateString(),
    };

    var meetingSearchFilter = {
      key: "meetingName",
      value: ''
    };

    this.dataSource.filter = [];
    this.dataSource.filter.push(filter);
    this.dataSource.filter.push(meetingSearchFilter);
    //this.loader.block();
    return this.gattingService.getAllMeetings(this.dataSource);
  }

  public ngOnInit(): void {
    this.getAllMeetings().subscribe(response => {
      //this.loader.unblock()

      this.data = response.data[0].meetings; 
    }, err => {
    });
    /*this.data = editingData;*/
    this.taskSettings = {
      id: 'id',
      name: 'name',
      startDate: 'from',
      endDate: 'to',
      notes: 'notes',
    };
    //this.resourceFields = {
    //  id: 'resourceId',
    //  name: 'resourceName'
    //};
    //this.editSettings = {
    //  allowAdding: true,
    //  allowEditing: true,
    //  allowDeleting: true,
    //  allowTaskbarEditing: true,
    //  showDeleteConfirmDialog: true
    //};
    /* this.toolbar = ['Add', 'Edit', 'Update', 'Delete', 'Cancel', 'ExpandAll', 'CollapseAll', 'Indent', 'Outdent'];*/
    this.toolbar = ['ZoomIn', 'ZoomOut', 'ZoomToFit'];
    this.columns = [
      { field: 'id', width: 80 },
      { field: 'name', headerText: 'Meeting Name', },
      //{ field: 'notes', headerText: 'Notes' },
      //{ field: 'meetingTopic.name', headerText: 'Meeting Topic' },
      //{ field: 'location', headerText: 'Location' },
      //{ field: 'project', headerText: 'Project' }
    ];
    //this.timelineSettings = {
    //  timelineViewMode: 'Hour',

    //  topTier: {
    //    unit: 'Day',
    //    format: 'EEEE dd-MM-yyyy',

    //  },
    //  bottomTier: {
    //    unit: 'Hour',
    //    format: 'hh',
    //    count: 4
    //  },

    //};

    this.timelineSettings = {
      timelineUnitSize: 65,
      topTier: {
        unit: 'Day',
        format: 'MMM dd, yyyy'
      },
      bottomTier: {
        unit: 'Hour',
        format: 'hh:mm a'
      }
    };
    this.dayWorkingTime = [{ from: 8, to: 19 }];
    this.workWeek = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Sunday'];


    this.gridLines = 'Both';
    //this.labelSettings = {
    //  leftLabel: 'TaskName',
    //  rightLabel: 'resources'
    //};
    //this.projectStartDate = new Date('03/25/2019');
    //this.projectEndDate = new Date('07/28/2019');
    //this.eventMarkers = [
    //  { day: '4/17/2019', label: 'Project approval and kick-off' },
    //  { day: '5/3/2019', label: 'Foundation inspection' },
    //  { day: '6/7/2019', label: 'Site manager inspection' },
    //  { day: '7/16/2019', label: 'Property handover and sign-off' },
    //];
    /*this.resources = editingResources;*/
    this.splitterSettings = {
      columnIndex: 2
    };
  }

}
