import { Component, ViewEncapsulation, ViewChild } from "@angular/core";
import { LocationService } from "../../services/location.service";
import { LocationModel } from "../../models/location.model";
import { ModalBasicComponent } from "../../../../../shared/components/modal-basic/modal-basic.component";
import { DataSource } from "src/app/shared/models/data-source.model";
import { LocationFormComponent } from '../location-form/location-form.component';
import { AlertService } from 'src/app/shared/services/alert.service';
import { AbilityService } from 'src/app/shared/services/ability.service';

@Component({
  selector: "app-location-list",
  templateUrl: "./location-list.component.html",
  styleUrls: ['../../../lookups-style.scss']
})
export class LocationListComponent {
  locationList: Array<LocationModel>; //Data List
  properties = ["name", "nameAr", "description"]; //Displayed Columns
  location: LocationModel = new LocationModel(); //For Add/Update Location Entity
  @ViewChild(LocationFormComponent) locationFormComponent;//Add/Update Role model
  dataSource: DataSource = new DataSource;
  total: number;
  link:any;
  constructor(private locationService: LocationService, private alertService: AlertService
    , public ability: AbilityService) { }

  ngOnInit() {
    this.link.action='Create';
    this.link.mod = 'location';
    this.getAllLocations();
  }

  getAllLocations() {
    this.locationService.getAll(this.dataSource).subscribe(
      response => {
        this.locationList = response.data;
        this.total = response.count;
      },
      err => { }
    );
  }

  public getLocationById(id: number) {
    this.locationService.get(id).subscribe(response => {
      this.location = response.data;
      this.locationFormComponent.open(this.location);
    }),
      error => { };
  }

  public save(entity) {
    this.locationService.save(entity).subscribe(response => {
      this.getAllLocations();
      this.alertService.handleResponseMessage(response);
    }),
      error => { };
  }

  public delete(id: number) {
    this.locationService.delete(id).subscribe(response => {
      this.getAllLocations();
      this.alertService.handleResponseMessage(response);
      
    }),
      error => { };
  }

  public openModal(id?: number) {
    if (id) {
      this.getLocationById(id);
    } else {
      this.location = new LocationModel();
      this.locationFormComponent.open(this.location);
    }
  }

  public onChangePagination(dataSource) {
    this.dataSource = dataSource;
    this.getAllLocations();
  }
}
