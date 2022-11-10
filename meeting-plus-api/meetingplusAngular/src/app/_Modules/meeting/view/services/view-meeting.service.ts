import { Injectable } from "@angular/core";
import { BaseEntityService } from "src/app/shared/services/base-entity.service";
import { Observable } from 'rxjs';
import { HttpParams } from '@angular/common/http';

@Injectable()
export class ViewMeetingService extends BaseEntityService {
  baseUrl: string;
  entityName = "Meeting";

  addNote(id: any, description: any): any {
    
    let note = {
      meetingID: id,
      description: description
    };
    return this._http.post(`${this.baseUrl}/${this.entityName}/AddNote`, note);
  }
}
