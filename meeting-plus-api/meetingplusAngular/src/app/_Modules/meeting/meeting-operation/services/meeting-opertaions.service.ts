import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { environment } from "src/environments/environment";
import { BaseEntityService } from "src/app/shared/services/base-entity.service";
import { Observable } from "rxjs";

@Injectable({
  providedIn: "root"
})
export class MeetingOperationsService extends BaseEntityService {
  baseUrl: string;
  entityName = "Meeting";

  public getAll(dataSource?): any {
    return this._http.get(`${this.baseUrl}/${this.entityName}/${this.urlGetAll}`, dataSource);
  }

  public getFiltredMeeting(dataSource?): any {
    return this._http.post(`${this.baseUrl}/${this.entityName}/GetFiltredMeeting`, dataSource);
  }

  public getAllLite(): any {
    return this._http.get(`${this.baseUrl}/${this.entityName}/GetAllMeetingLite`);
  }

  getAllMeetings(dataSource: any): Observable<any> {
    return this._http.post(
      `${this.baseUrl}/${this.entityName}/GetAllMeetings`,
      dataSource
    );
  }

  start(id: any): any {
    return this._http.post(`${this.baseUrl}/${this.entityName}/Start?ID=` + id, null);
  }

  end(id: any): any {
    return this._http.post(`${this.baseUrl}/${this.entityName}/End?ID=` + id, null);
  }

  join(id: any): any {
    // const params = new HttpParams().set('meetingId', id);
    return this._http.post(`${this.baseUrl}/${this.entityName}/Join?MeetingId=` + id, null);
  }

  accept(id: any): any {
    return this._http.post(`${this.baseUrl}/${this.entityName}/Accept?ID=` + id, null);
  }

  decline(id: any): any {
    return this._http.post(`${this.baseUrl}/${this.entityName}/Decline?ID=` + id, null);
  }

  propose(id: any, purposedTo: string): any {
    return this._http.post(
      `${this.baseUrl}/${this.entityName}/ProposeNewTime?ID=${id}&PurposedTo=${purposedTo}`, null
    );
  }

  getParticipants(meetingStart: any, meetingEnd: any, meetingId?: any | null): any {
    if (meetingId) {
      return this._http.post(
        `${this.baseUrl}/${this.entityName}/GetParticipants?MeetingStart=${meetingStart}&MeetingEnd=${meetingEnd}&MeetingId=${meetingId}`, {}
      );
    }
    return this._http.post(
      `${this.baseUrl}/${this.entityName}/GetParticipants?MeetingStart=${meetingStart}&MeetingEnd=${meetingEnd}`, {}
    );
  }

  getLocations(meetingStart: any, meetingEnd: any, meetingId?: any | null): any {
    if (meetingId) {
      return this._http.post(
        `${this.baseUrl}/${this.entityName}/GetLocations?MeetingStart=${meetingStart}&MeetingEnd=${meetingEnd}&MeetingId=${meetingId}`,
        {}
      );
    }
    return this._http.post(
      `${this.baseUrl}/${this.entityName}/GetLocations?MeetingStart=${meetingStart}&MeetingEnd=${meetingEnd}`,
      {}
    );
  }

  addParticipants(meetingId: any, participants: any) {
    return this._http.post(`${this.baseUrl}/${this.entityName}/AddParticipants?ID=${meetingId}`, participants)
  }



  //Copy Link
}
