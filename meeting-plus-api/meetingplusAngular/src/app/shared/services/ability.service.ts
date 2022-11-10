import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AuthService } from 'src/app/_Modules/auth/services/auth.service';



@Injectable({ providedIn: 'root' })
export class AbilityService {

  constructor(private authService: AuthService) { }

  public get() {
    return this.authService.currentUserValue;
  }

  can(action: string, mod: string) {
    return this.get() && (this.get().isSuperAdmin || (this.get() && this.get().ability && this.get().ability.filter(x => x.fkModule.name.toLowerCase() == mod
      && x.fkPrivilege.name.toLowerCase() == action).length > 0));
  }


}
