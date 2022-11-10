import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { WorkingDay, weekDays } from '../../models/working-day';
import { SystemConfigurationService } from 'src/app/_Modules/system-configuration/services/system-configuration.service';
@Component({
  selector: 'app-working-days',
  templateUrl: './working-days.component.html',
  styleUrls: ['./working-days.component.scss']
})

export class WorkingDaysComponent implements OnInit {

  week: Array<WorkingDay>; //the working week of the company
  currentDate: Date;
  @Output() selectedDay = new EventEmitter<Date>();//When you edit row this will send selected (Id) for parent component

  constructor(private systemConfigService: SystemConfigurationService) {
    this.currentDate = new Date();
    this.week = [];
  }

  ngOnInit() {
    this.getSystemConfiguration();
  }

  private getSystemConfiguration() {
    this.systemConfigService.get().subscribe(response => {
      this.getWeekDays(response.data.startOfWorkDays, response.data.endOfWorkDays);
      this.onSelecteDay(new Date());//Passing current date in the first time
    }, err => {
    });
  }

  getWeekDays(startDay: number, endDay: number) {
    //this function is used to get the days in the current week 
    for (let i = startDay; i <= endDay; i++) {
      let dayDate = this.getDayDate(this.currentDate, i);
      this.week.push(new WorkingDay(weekDays[i], dayDate));
    }
  }

  private getDayDate(currentDate, index) {
    let firstDayOfWeek = currentDate.getDate() - currentDate.getDay() + index;
    return new Date(currentDate.setDate(firstDayOfWeek)).toISOString().slice(0, 10);
  }

  public onSelecteDay(day: any) {
    if (day.date) {
      this.selectedDay.emit(new Date(day.date));
      this.activateSelectedDayTab(new Date(day.date));
    }
    else {
      this.selectedDay.emit(new Date(day));
      this.activateSelectedDayTab(new Date(day));
    }
  }

  activateSelectedDayTab(selectedDate) {

    for (let day of this.week) {
      if (selectedDate.toDateString() == new Date(day.date).toDateString()) {
        day.isActive = true;
      }
      else {
        day.isActive = false;
      }
    }
  }
}
