import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router, ActivatedRoute, NavigationEnd, RoutesRecognized } from '@angular/router';
import { filter, pairwise } from 'rxjs/operators';
import { RoutingStateService } from '../../../../shared/services/routing-state.service';
import { ToastrService } from 'ngx-toastr';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  LoginForm: FormGroup;
  submited: boolean;
  loginSuccess: boolean;
  loginFalied: boolean;
  previousUrl: string;//To go to the last visited page
  isInValidCredential: boolean = false;//Show an error message in case invalid Username of Password
  constructor(private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private activeRoute: ActivatedRoute,
    private toaster: ToastrService,
    private routingStateService: RoutingStateService,
    private translate: TranslateService) {
    this.initForm();
    this.submited = false;
  }

  ngOnInit() {
    this.previousUrl = this.routingStateService.getPreviousUrl();//Getting the last url
  }

  private initForm() {
    this.LoginForm = this.fb.group({
      email: ["", [Validators.required]],
      password: ["", [Validators.required]]
    })
  }

  public login() {
    this.submited = true;
    this.isInValidCredential = false;
    this.authService.login(this.LoginForm.value)
      .subscribe((response) => {

        if (response.success) {
          debugger;
          localStorage.removeItem("lang");
          localStorage.setItem("lang", "en");
          this.authService.setCurrentUser(JSON.stringify(response.data));
          this.authService.setToken(JSON.stringify(response.data.token));
          if (!this.previousUrl) { this.previousUrl = "/Meetings"; }
          this.loginSuccess = true;
          setTimeout(() => {
            this.router.navigate([this.previousUrl]);
          }, 2000);

        }
        else {
          this.loginFalied = true;
          this.toaster.error(this.translate.instant(response.message), "Login Failed");

          setTimeout(() => {
            this.submited = false;
            this.loginFalied = false;
          }, 2000);
          //this.isInValidCredential = true;
        }
      })
      , error => {
        this.loginFalied = true;
        setTimeout(() => {
          this.submited = false;
          this.loginFalied = false;
        }, 2000);

      }
  }
}