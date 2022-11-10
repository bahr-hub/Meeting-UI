import { Component, ViewEncapsulation, ViewChild } from "@angular/core";
import { UserService } from "../../services/user.service";
import { UserModel } from "../../models/user.model";
import { DataSource } from "../../../../shared/models/data-source.model";
import { ModalBasicComponent } from '../../../../shared/components/modal-basic/modal-basic.component';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { AlertService } from 'src/app/shared/services/alert.service';
import { AbilityService } from 'src/app/shared/services/ability.service';

@Component({
  selector: "app-user-list",
  templateUrl: "./user-list.component.html",
  styleUrls: ['../../../lockups/lookups-style.scss']
})
export class UserListComponent {
  userList: Array<UserModel>; //Data List
  properties = ["name", "email", "mobile","isActive"]; //Displayed Columns
  user: UserModel = new UserModel(); //For Add/Update User Entity
  @ViewChild(ModalBasicComponent) modalBasicComponent; //Add/Update User model
  dataSource: DataSource = new DataSource;
  total: number;

  constructor(private userService: UserService, private router: Router, private toastr: ToastrService, 
    private alertService: AlertService, public ability: AbilityService) { }

  ngOnInit() {
    this.getAllUsers();
  }

  getAllUsers() {
    this.userService.getAll(this.dataSource).subscribe(
      response => {
        debugger
        this.userList = response.data;
        this.total = response.count;
      },
      err => { }
    );
  }

  add()
  {
    this.router.navigateByUrl('/users/user-form')
  }
 


  public save(entity) {
    this.userService.save(entity).subscribe(response => {
      this.getAllUsers();
    }),
      error => { };
  }

  public delete(id) {
    this.userService.delete(id).subscribe(response => {
      this.getAllUsers();
      this.alertService.handleResponseMessage(response);
    }),
      error => { };
  }
  public changeStatus(val) {
    debugger;
    this.userService.changeStatus(val).subscribe(response => {
      this.getAllUsers();
      this.alertService.handleResponseMessage(response);
    }),
      error => { };
  }
  
  public edit(id: number) {
    this.router.navigate(["/users/user-form", id]);
  }

  public onChangePagination(dataSource) {
    this.dataSource = dataSource;
    this.getAllUsers();
  }
}
