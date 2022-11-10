import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { BaseEntityService } from 'src/app/shared/services/base-entity.service';
import { Observable } from 'rxjs';

@Injectable()
export class GattingService extends BaseEntityService {
    baseUrl: string;
    entityName = "Meeting";

  getAllMeetings(dataSource: any): Observable<any> {
    return this._http.post(`${this.baseUrl}/${this.entityName}/GetAllMeetings?isGatting=true`, dataSource);
    }

    //getMeetingTasks(dataSource: any): Observable<any> {
    //    return this._http.post(`${this.baseUrl}/MeetingTask/GetAll`, dataSource);
    //}

    //closeTask(id: any): Observable<any> {
    //    return this._http.post(`${this.baseUrl}/MeetingTask/CloseTask?ID=` + id, null);
    //}
}
