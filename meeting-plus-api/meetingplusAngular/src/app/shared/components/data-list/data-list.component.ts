
import { Component, OnInit, Inject, Input, EventEmitter, Output, ViewChild } from '@angular/core';
import { ConfirmationModalComponent } from 'src/app/shared/components/confirmation-dialog/confirmation-modal.component';
import { ModalBasicComponent } from 'src/app/shared/components/modal-basic/modal-basic.component';
import { DataSource } from 'src/app/shared/models/data-source.model';
import { AbilityService } from 'src/app/shared/services/ability.service';

@Component({
  selector: 'app-data-list',
  templateUrl: './data-list.component.html',
  styleUrls: ['./data-list.component.scss']
})

export class DataListComponent {
  //#region  Description:
  /*
 [M.Salman]
  this is a generic component for listing data
  */
  //#endregion
  //#region [M.Salman] -[30/1/2020] - [Variables]
  @Input() list;//List(table content)
  @Input() properties;//Table Header
  @Output() deleteRow = new EventEmitter<number>();//When you delete row this will send selected (Id) for parent component
  @Output() changeStatus = new EventEmitter<any>();//When you changeStatus row this will send selected (Id) for parent component
  @Output() editRow = new EventEmitter<number>();//When you edit row this will send selected (Id) for parent component
  @Output() changePagination = new EventEmitter<DataSource>();//When you edit row this will send selected (Id) for parent component
  @ViewChild(ConfirmationModalComponent) confirmationModalComponent;//This open dialog for confirmation delete
  @Input() total: number;
  dataSource = new DataSource;
  model: any = {};
  @Input() pageName:string;
  //#endregion

  constructor( public ability: AbilityService) {
  }

  ngOnInit() {
   
  }

  showConfirmDialog(id): void {
    this.confirmationModalComponent.open(id);//Open dialog when you press in delete button
  }

  public delete(id) {
    this.deleteRow.emit(id);//Send selected id to parent after confirmation delete
  }

  public change_Status(id,isActive) {
    isActive=!isActive;
   this.changeStatus.emit({id,isActive});
  }
  
  public edit(id) {
    this.editRow.emit(id);//Send selected id to parent after editing the entity
  }

  onChangePagination(data) {
    this.dataSource.pageSize = data.recordPerPage;
    this.dataSource.page = data.currentPage;
    this.changePagination.emit(this.dataSource);
  }

  public applyFilter() {
    this.dataSource.filter = [];
    for (let property of this.properties) {
      if (this.model[property]) {
        var filter = {
          Key: this.titleCaseWord(property),
          value: this.model[property]
        };
        this.dataSource.filter.push(filter);
      }

    }
    this.changePagination.emit(this.dataSource);
  }

  public titleCaseWord(word: string) {
    if (!word) return word;
    return word[0].toUpperCase() + word.substr(1);
  }

}


