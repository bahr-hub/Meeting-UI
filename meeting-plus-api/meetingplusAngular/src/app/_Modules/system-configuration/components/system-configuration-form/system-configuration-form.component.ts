import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { SystemConfigurationService } from '../../services/system-configuration.service';
import { SystemConfigurationModel } from '../../models/system-configuration.model';
import { CountryService } from 'src/app/_Modules/user/services/country.service';
import { WeekDaysEnum } from 'src/app/shared/enums/week-days.enum';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-system-configuration-form',
  templateUrl: './system-configuration-form.component.html',
  styleUrls: ['./system-configuration-form.component.css']
})
export class SystemConfigurationFormComponent implements OnInit {

  //#region  [Author:Mahmoud Ali] - [24-2-2020] - [Declaring of variables]
  systemConfigurationModel: SystemConfigurationModel = new SystemConfigurationModel;
  timeZones = [];
  weekDays = [];
  //#endregion

  constructor(
    private toastr: ToastrService,
    private router: Router,
    private systemConfigurationService: SystemConfigurationService,
    private countryService: CountryService,
    public translate: TranslateService
  ) {

  }

  ngOnInit() {
    this.getSystemConfigurationById();
    this.getTimezoneOffset();
    this.prepareWeekDaysList();
  }

  public getSystemConfigurationById() {
    this.systemConfigurationService.get()
      .subscribe((response) => {
        this.systemConfigurationModel = response.data;
        // this.systemConfigurationModel.timeZone = this.systemConfigurationModel.timeZone.split(',')[1];
        // this.accountTreeType = AccountTreeEnum[this.safeModel.AccountTreeType];

      })
      , error => {
      }
  }

  public save(form: NgForm) {
    this.systemConfigurationService.save(this.systemConfigurationModel)
      .subscribe((response) => {
        if (response.success) {
          this.toastr.success(response.message);
          this.router.navigate(['/system-configurations']);
        }
        else {
          this.toastr.error(response.message);
        }
      })
      , error => {
      }
  }

  private getTimezoneOffset() {
    this.countryService.getAllCountries('https://restcountries.eu/rest/v2/all')
      .subscribe(res => {
        var countries: any = [];
        countries = res;
        this.timeZones = countries.map(x => x.timezones);

      })
  }

  private prepareWeekDaysList() {
    var options = Object.keys(WeekDaysEnum);
    let days = options.slice(options.length / 2);
    for (let i = 0; i < days.length; i++) {
      let day = {
        key: i,
        value: days[i]
      };
      this.weekDays.push(day);
    }
  }


}
