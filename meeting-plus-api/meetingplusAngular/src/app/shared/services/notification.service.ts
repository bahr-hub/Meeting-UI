import { Injectable } from "@angular/core";
import { BaseEntityService } from "src/app/shared/services/base-entity.service";
import { analyzeAndValidateNgModules } from '@angular/compiler';

@Injectable()
export class NotificationService extends BaseEntityService {
  baseUrl: string;
  entityName = "Notification";
 
  get(id): any {
    return this._http.get(`${this.baseUrl}/${this.entityName}/Get?ID=`+ id,null);

  }

}
