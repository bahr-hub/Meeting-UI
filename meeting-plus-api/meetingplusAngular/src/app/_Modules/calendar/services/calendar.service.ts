import { BaseEntityService } from 'src/app/shared/services/base-entity.service';
import { Injectable } from '@angular/core';

@Injectable()
export class CalendarService extends BaseEntityService {
    entityName = "Meeting";

    getInterval(start: any, end: any): any {
        return this._http.get(`${this.baseUrl}/${this.entityName}/GetInterval`, {
            params: {
                start: start,
                end: end
            }
        });
    }

}