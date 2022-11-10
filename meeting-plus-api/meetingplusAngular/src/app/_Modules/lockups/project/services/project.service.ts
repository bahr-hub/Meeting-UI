import { Injectable } from '@angular/core';
import { BaseEntityService } from '../../../../shared/services/base-entity.service';

@Injectable(
    {
        providedIn:"root"}
)
export class ProjectService extends BaseEntityService {
    entityName = "Project";
}