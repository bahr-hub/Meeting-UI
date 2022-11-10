import { Component, ViewChild, EventEmitter, Output, Input } from '@angular/core';
import { NgbModal, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-modal-basic',
  templateUrl: './modal-basic.component.html'
})

export class ModalBasicComponent {
  //#region  Description 
  /*
  This is a generic component for generating form for input controls (Lockups pages)
  */

  //#region  [M.Salman]-[30-1-2020] - [Variables]
  @ViewChild('content') content: any;//Displaying of modal
  @Output() saveRow = new EventEmitter<any>();//Sending of object to parent after add or update
  model: any = {};//model that bind on passing entity
  @Input() properties: any;//properties names
  //#endregion

  constructor(private modalService: NgbModal) { }

  public open(model: any) {
    // this.properties = Object.keys(model).filter(x => x != 'Id');//Getting all properties except Id
    this.model = model;
    this.modalService.open(this.content);
  }

  public save(form: NgForm) {
    this.saveRow.emit(this.model);//Passing of entity to parent component
    this.modalService.dismissAll();
  }

}
