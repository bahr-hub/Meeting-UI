import { BaseEntityService } from 'src/app/shared/services/base-entity.service';
import { Injectable } from '@angular/core';

@Injectable()
export class UserService extends BaseEntityService {
    entityName = "User";

    getByToken(): any {
        return this._http.get(this.baseUrl + 'api/' + this.entityName + '/GetByToken');
    }

    uploadPhoto(userId: number): any {
        return this._http.get(this.baseUrl + "api/" + this.entityName + '/UploadPhoto/' + userId);
    }

    register(User: any): any {
        return this._http.get(this.baseUrl + 'api/' + this.entityName + '/Register', User);
    }

    changeUserPassword(User: any): any {
        return this._http.get(this.baseUrl + "api/" + this.entityName + '/ChangeUserPassword/' + User);
    }

    getUserProfile(id: number): any {
        return this._http.get(this.baseUrl + "api/" + this.entityName + '/GetUserProfile/' + id);
    }
    public integrate(): any {
        debugger;
        return this._http.get(this.baseUrl + "/" + this.entityName + '/Integrate');
    }
    // public delete(id) {
    //     return this._http.delete(`${this.baseUrl}${this.entityName}/${this.urlDelete}?id=${id}`);
    // }
}