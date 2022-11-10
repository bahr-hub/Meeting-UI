import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { DomSanitizer } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { CountryService } from '../../services/country.service';
import { RoleService } from '../../../lockups/role/services/role.service';
import { LocationService } from '../../../lockups/location/services/location.service';
import { TranslateService } from '@ngx-translate/core';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../../auth/services/auth.service';
import { UserModel } from '../../models/user.model';
import { AlertService } from 'src/app/shared/services/alert.service';
import { environment } from 'src/environments/environment';
import { DataSource } from 'src/app/shared/models/data-source.model';

@Component({
  selector: 'app-user-form',
  templateUrl: './user-form.component.html',
  styleUrls: ['./user-form.component.scss']
  // styleUrls: ['./user-form.component.scss']
})
export class UserFormComponent implements OnInit {

  //#region  [Author:Mahmoud Ali] - [7-10-2019] - [Declaring of variables]
  hide = true;
  apiPhoto;
  currentUser: any;
  photoPath;
  userModel: UserModel = new UserModel;
  photoPlaceholder: string = 'assets/imgs/avatar.png'
  id: any;
  userName = new FormControl('', [Validators.required]);
  countries: any = [];//Countries select
  rolesList: any = [];//Role select
  timeZones: any;
  formData: FormData;
  selectedCountry: any;
  public selectedRole: number;
  public selectedLocation:number;
  imageUrl: string = null;
  isFromHeader: boolean = false;
  locations: any;
  dataSource: DataSource = new DataSource;
  //#endregion

  constructor(private activatedRoute: ActivatedRoute,
    private sanitizer: DomSanitizer,
    private toastr: ToastrService,
    private router: Router,
    private countryService: CountryService,
    private roleService: RoleService,
    private userService: UserService,
    private authService: AuthService,
    private alertService: AlertService,
    private locationService: LocationService,
    public translate: TranslateService) {

  }

  ngOnInit() {
    this.id = this.activatedRoute.snapshot.params["id"];
    this.isFromHeader = this.activatedRoute.snapshot.params["fromHeader"];
    if (this.id) {
      this.getUserById(this.id);
    }
    else {
      this.getAllCountries();
    }
    this.getAllRoles();
    
    this.getAllLocations();
  }

  public getUserById(id: number) {
    this.userService.get(id)
      .subscribe((response) => {
     
        
        this.userModel = response.data;
        this.userModel.password = null;
        this.imageUrl = environment.imageUrl + this.userModel.imageUrl + ".png";
        this.setTimezone();
        this.setSelectedRole();
        this.getAllCountries();
       this.setSelectedLocation();
      })
      , error => {
      }
  }
  private setTimezone() {
    this.userModel.fkUserConfiguration.timeZone = this.userModel.fkUserConfiguration.timeZone.split(',')[1];
  }
  private setSelectedLocation() {  debugger;
    if (this.userModel.locationID) {
      this.selectedLocation = this.userModel.locationID;
    }
 
  
  }
 private setSelectedRole() {

    if (this.userModel.userRole) {
      this.selectedRole = this.userModel.userRole[0].fkRoleId;
    }
  }
  getAllRoles() {
    this.roleService.getAllLite().subscribe(response => {
      this.rolesList = response.data;
      if (!this.id)
        this.selectedRole = null;
    })
  }
  getAllLocations() {
 this.dataSource.pageSize = 500;
    this.locationService.getAll(this.dataSource).subscribe(response => {
    
      this.locations = response.data;
      if (!this.id)
        this.selectedLocation = null;
       
    })
  }

  getAllCountries() {

    this.countryService.getAllCountries('https://restcountries.eu/rest/v2/all')
      .subscribe(res => {
        this.countries = res;
        let country = this.countries.find(x => x.alpha3Code == this.userModel.fkUserProfile.countryCode);
        if (country) {
          this.timeZones = country.timezones;
        }
      })
  }

  public onCountryChange(selectedAlpha3Code: any) {
    debugger;
    if (this.countries && this.countries.length > 0 && selectedAlpha3Code && selectedAlpha3Code != '') {
      this.timeZones = this.countries.find(x => x.alpha3Code == selectedAlpha3Code).timezones;
      if (this.timeZones) {
        this.userModel.fkUserConfiguration.timeZone = this.timeZones[0];
      }
    }



   }

  public numberOnly(event): boolean {
    const charCode = (event.which) ? event.which : event.keyCode;
    if (charCode == 43) {
      return true;
    }
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
      return false;
    }
    return true;

  }


  public save(form: NgForm) {
    if (form.invalid || this.userModel.password != this.userModel.confirmPassword) {
      return;
    }
    this.prepareUserRole();
    this.userService.save(this.userModel)
      .subscribe((response) => {
        if (this.alertService.handleResponseMessage(response))
          this.router.navigate(['/users']);
      })
      , error => {
      }
  }

  private prepareUserRole() {
    this.userModel.userRole = [];
    // for (let item of this.selectedRoles) {
    let userRole = {
      FkUserId: this.userModel.id,
      FkRoleId: this.selectedRole
    };
    this.userModel.userRole.push(userRole);
    
    this.userModel.locationID = this.selectedLocation;
    // }
  }

  public onImageChange(imageBase64) {
    this.userModel.imageBase64 = imageBase64.split("base64,")[1];
  }
  public GiveAccess() {
    this.userService.integrate().subscribe((response) => {
      this.alertService.handleResponseMessage(response)
    });

  }

}
