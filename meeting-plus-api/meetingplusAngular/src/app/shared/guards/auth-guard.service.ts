import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';
import { Router } from '@angular/router';
import { JwtHelperService } from "@auth0/angular-jwt";
import { AuthService } from 'src/app/_Modules/auth/services/auth.service';

@Injectable(
   
)
export class AuthGuardService implements CanActivate {
    constructor(private jwtHelper: JwtHelperService, private router: Router, private authService: AuthService) {
    }
    canActivate() {
        var token =this.authService.getToken();

        if (token && !this.jwtHelper.isTokenExpired(token)) {
            return true;
        }
        this.router.navigate(["Auth/Login"]);
        return false;
    }

}