import { Injectable } from "@angular/core";
import { BaseEntityService } from "src/app/shared/services/base-entity.service";


@Injectable({providedIn:'root'})
export class MeetingTaskService extends BaseEntityService {
  baseUrl: string;
  entityName = "MeetingTask";

  Add(Task: any): any {
    
    return this._http.post(this.baseUrl  +'/'+ this.entityName + '/Create', Task);
}

CloseTask(id: any): any {
  return this._http.post(`${this.baseUrl}/${this.entityName}/CloseTask?ID=`+ id,null);
  
}

}
