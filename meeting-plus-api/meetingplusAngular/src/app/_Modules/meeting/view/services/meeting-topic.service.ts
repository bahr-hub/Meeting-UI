import { Injectable } from "@angular/core";
import { BaseEntityService } from "src/app/shared/services/base-entity.service";


@Injectable()
export class MeetingTopicService extends BaseEntityService {
  baseUrl: string;
  entityName = "MeetingTopic";
  
  Add(Topic: any): any {
    
    return this._http.post(this.baseUrl  + this.entityName + '/Create', Topic);
}

CloseTopic(id: any): any {
  return this._http.post(`${this.baseUrl}/${this.entityName}/CloseTopic?ID=`+ id,null);
  
}

DeleteTopic(id: any): any {
  return this._http.delete(`${this.baseUrl}${this.entityName}/DeleteMeetingTopic?ID=${id}`);

}


}
