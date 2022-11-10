import { Injectable } from '@angular/core';
import { BaseEntityService } from '../../../../shared/services/base-entity.service';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class RoleService extends BaseEntityService {

    entityName = "Role";

    getModules(dataSource: any): any {
        return this._http.post(`${this.baseUrl}/${this.entityName}/GetModules`, dataSource);
    }

    getPrivileges(dataSource): any {
        return this._http.post(`${this.baseUrl}/${this.entityName}/GetPrivileges`, dataSource);
    }


}