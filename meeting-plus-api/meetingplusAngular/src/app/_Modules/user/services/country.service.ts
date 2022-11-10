import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class CountryService {

    constructor(private _http: HttpClient) {
    }

    getAllCountries(API: string) {
        return this._http.get(API);
    }
}