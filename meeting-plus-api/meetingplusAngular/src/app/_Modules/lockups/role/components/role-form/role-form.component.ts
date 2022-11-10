import { Component, ViewChild, EventEmitter, Output, Input } from '@angular/core';
import { NgbModal, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';
import { NgForm } from '@angular/forms';
import { DataSource } from '../../../../../shared/models/data-source.model';
import { RoleService } from '../../services/role.service';
import { RoleModel } from 'src/app/_Modules/lockups/role/models/role.model';
import { RoleModulePrivilege } from 'src/app/_Modules/lockups/role/models/role-module-privilege';

@Component({
  selector: 'app-role-form',
  templateUrl: './role-form.component.html'
})

export class RoleFormComponent {
  //#region  [M.Salman]-[12-2-2020] - [Variables]
  @ViewChild('content') content: any;//Displaying of modal
  @Output() saveRow = new EventEmitter<any>();//Sending of object to parent after add or update
  model: RoleModel = new RoleModel;//model that bind on passing entity
  modules;
  //#endregion

  constructor(private modalService: NgbModal, private roleService: RoleService) { }

  ngOnInit() {

  }

  public open(model: any) {
    // this.properties = Object.keys(model).filter(x => x != 'Id');//Getting all properties except Id
    this.model = model;
    this.getModules();
    this.modalService.open(this.content);
  }

  public save(form: NgForm) {
    if (form.invalid) {
      return;
    }
    this.saveRow.emit(this.model);//Passing of entity to parent component
    this.modalService.dismissAll();
  }

  getModules() {
    let dataSource = new DataSource();
    this.roleService.getModules(dataSource)
      .subscribe(res => {
        this.modules = res.data;
      });
  }

  public onSelectPrivilege(selectedPrivilege) {
    let privilege = this.prepareRoleModulePrivilegeEntity(selectedPrivilege);
    let found = this.model.roleModulePrivilege.findIndex(obj => obj.fkModuleId == privilege.fkModuleId && obj.fkPrivilegeId == privilege.fkPrivilegeId)
    found < 0 ? this.model.roleModulePrivilege.push(privilege) : this.model.roleModulePrivilege.splice(found, 1);
  }

  private prepareRoleModulePrivilegeEntity(selectedPrivilege) {
    //Preparing of RoleModule entity
    let privilege = new RoleModulePrivilege();
    privilege.fkPrivilegeId = selectedPrivilege.fkPrivilegeId;
    privilege.fkModuleId = selectedPrivilege.fkModuleId;
    return privilege;
  }

  public isPrivilageInRole(privilege) {
    if (!this.model)
      return;
    //to be fixed later
    let index = this.model.roleModulePrivilege.findIndex(obj => obj.fkModuleId == privilege.fkModuleId && obj.fkPrivilegeId == privilege.fkPrivilegeId);
    return index < 0 ? false : true;
  }

  public getTranslatedModule(moduleName) {
    return `roles.modules.${moduleName}`;
  }
}
