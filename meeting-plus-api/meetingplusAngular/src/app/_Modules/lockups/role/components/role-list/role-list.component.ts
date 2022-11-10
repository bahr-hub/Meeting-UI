import { Component, ViewEncapsulation, ViewChild } from '@angular/core';
import { RoleService } from '../../services/role.service';
import { RoleModel } from '../../models/role.model';
import { DataSource } from '../../../../../shared/models/data-source.model';
import { RoleFormComponent } from '../role-form/role-form.component';
import { ToastrService } from 'ngx-toastr';
import { AlertService } from 'src/app/shared/services/alert.service';
import { AbilityService } from 'src/app/shared/services/ability.service';

@Component({
  selector: 'app-role-list',
  templateUrl: './role-list.component.html',
  styleUrls: ['../../../lookups-style.scss']
})

export class RoleListComponent {
  roleList = [];
  role: RoleModel = new RoleModel();
  properties = ["name", "description"];//Displayed Columns 
  @ViewChild(RoleFormComponent) roleFormComponent;//Add/Update Role model
  dataSource: DataSource = new DataSource;
  total: number;

  constructor(private roleService: RoleService, private toastr: ToastrService, private alertService: AlertService, public ability: AbilityService) {

  }

  ngOnInit() {
    this.getAllRoles();
  }

  getAllRoles() {
    this.roleService.getAll(this.dataSource).subscribe(response => {
      this.roleList = response.data;
      this.total = response.count;
    }, err => {
    });
  }

  public getRoleById(id: number) {
    this.roleService.get(id)
      .subscribe((response) => {
        this.role = response.data;
        this.roleFormComponent.open(this.role);
      })
      , error => {
      }
  }

  public save(entity) {
    this.roleService.save(entity)
      .subscribe((response) => {
        this.getAllRoles();
        this.alertService.handleResponseMessage(response);
      })
      , error => {
      }
  }

  public delete(id: number) {
    this.roleService.delete(id)
      .subscribe((response: any) => {
        if (response.success) {
          this.getAllRoles();
          this.alertService.handleResponseMessage(response);
        }
        else {
          this.toastr.error(response.message)
        }
      })
      , error => {
      }
  }

  public openModal(id?: number) {
    if (id) {
      this.getRoleById(id);
    }
    else {
      this.role = new RoleModel();
      this.roleFormComponent.open(this.role);
    }
  }

  public onChangePagination(dataSource) {
    this.dataSource = dataSource;
    this.getAllRoles();
  }
}
