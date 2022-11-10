import { BaseEntityService } from 'src/app/shared/services/base-entity.service';
import { Injectable } from '@angular/core';

@Injectable()
export class SystemConfigurationService extends BaseEntityService {
    entityName = "SystemConfiguration";

    get(): any {
        return this._http.get(`${this.baseUrl}/${this.entityName}/Get`);
    }
}