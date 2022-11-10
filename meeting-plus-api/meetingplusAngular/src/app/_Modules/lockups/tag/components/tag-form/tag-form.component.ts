import { Component, ViewChild, EventEmitter, Output, Input } from '@angular/core';
import { NgbModal, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';
import { NgForm } from '@angular/forms';
import { DataSource } from '../../../../../shared/models/data-source.model';
import { TagService } from '../../services/tag.service';
import { TagModel } from 'src/app/_Modules/lockups/tag/models/tag.model';

@Component({
  selector: 'app-tag-form',
  templateUrl: './tag-form.component.html'
})

export class TagFormComponent {
  //#region  [M.Salman]-[12-2-2020] - [Variables]
  @ViewChild('content') content: any;//Displaying of modal
  @Output() saveRow = new EventEmitter<any>();//Sending of object to parent after add or update
  model: TagModel = new TagModel;//model that bind on passing entity
  //#endregion

  constructor(private modalService: NgbModal, private tagService: TagService) { }

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
