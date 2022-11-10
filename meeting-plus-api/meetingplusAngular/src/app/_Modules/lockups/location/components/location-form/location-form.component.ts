import { Component, ViewChild, EventEmitter, Output, Input } from '@angular/core';
import { NgbModal, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';
import { NgForm } from '@angular/forms';
import { DataSource } from '../../../../../shared/models/data-source.model';
import { LocationService } from '../../services/location.service';
import { LocationModel } from 'src/app/_Modules/lockups/location/models/location.model';

@Component({
  selector: 'app-location-form',
  templateUrl: './location-form.component.html'
})

export class LocationFormComponent {
  //#region  [M.Salman]-[12-2-2020] - [Variables]
  @ViewChild('content') content: any;//Displaying of modal
  @Output() saveRow = new EventEmitter<any>();//Sending of object to parent after add or update
  model: LocationModel = new LocationModel;//model that bind on passing entity
  //#endregion

  constructor(private modalService: NgbModal, private locationService: LocationService) { }

  ngOnInit() {

  }

  public open(model: any) {
    this.model = model;
    this.modalService.open(this.content);
  }

  public save(form: NgForm) {
    if (form.invalid) {
      return;
    }
    this.saveRow.emit(this.model);//Passing of entity to parent component
    this.modalService.dismissAll();
  }

}
