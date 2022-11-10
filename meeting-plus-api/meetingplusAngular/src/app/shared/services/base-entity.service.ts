
import { environment } from '../../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/internal/operators/catchError';
import { Injectable } from '@angular/core';

@Injectable()

export class BaseEntityService {
    protected baseUrl: string = "";
    protected urlGetAll = "GetAll";
    protected urlGet = "Get";
    protected urlCreate = "Create";
    protected urlUpdate = "Update";
    protected urlDelete = "Delete";
    protected urlGetAllLite = "GetAllLite";

    public entityName: string;
    constructor(public _http: HttpClient) {
        this.baseUrl = `${environment.apiUrl}api`;
    }


    public getAll(dataSource?): any {
     
        return this._http.post(`${this.baseUrl}/${this.entityName}/${this.urlGetAll}`, dataSource);
    }

    public getAllLite(): any {
        return this._http.get(`${this.baseUrl}/${this.entityName}/${this.urlGetAllLite}`);
    }

    public get(id): any {
        return this._http.get(`${this.baseUrl}/${this.entityName}/${this.urlGet}?ID=${id}`);
    }


    public save(entity): any {
        if (entity.id) {
            return this.update(entity);
        }
        else {
            return this.create(entity);
        }
    }
    
    public create(entity) :any{
        return this._http.post(`${this.baseUrl}/${this.entityName}/${this.urlCreate}/`, entity);
    }

    public update(entity):any {
        return this._http.put(`${this.baseUrl}/${this.entityName}/${this.urlUpdate}/`, entity);
    }

    public changeStatus(entity) {
        return this._http.post(`${this.baseUrl}/${this.entityName}/Change${this.entityName}Status/`, entity);
    }

    public delete(id): any {
        return this._http.delete(`${this.baseUrl}/${this.entityName}/${this.urlDelete}${this.entityName}?ID=${id}`);
    }

    private errorHandler(error: Response) {
        return Observable.throw(error);
    }
}
