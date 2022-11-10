import { Component, ViewChild, EventEmitter, Output, Input } from '@angular/core';
import { NgbModal, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';
import { NgForm } from '@angular/forms';
import { DataSource } from '../../../../../shared/models/data-source.model';
import { ProjectService } from '../../services/project.service';
import { ProjectModel } from 'src/app/_Modules/lockups/project/models/project.model';

@Component({
  selector: 'app-project-form',
  templateUrl: './project-form.component.html'
})

export class ProjectFormComponent {
  //#region  [M.Salman]-[12-2-2020] - [Variables]
  @ViewChild('content') content: any;//Displaying of modal
  @Output() saveRow = new EventEmitter<any>();//Sending of object to parent after add or update
  model: ProjectModel = new ProjectModel;//model that bind on passing entity
  //#endregion

  constructor(private modalService: NgbModal, private projectService: ProjectService) { }

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
