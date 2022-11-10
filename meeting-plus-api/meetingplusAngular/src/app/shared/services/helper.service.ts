import { Injectable, Inject } from '@angular/core';
import { DatePipe } from '@angular/common';


@Injectable()
export class HelperService {

    constructor(private _datePipe: DatePipe) {

    }
    transformDate(date) {
        return this._datePipe.transform(date, "yyyy-MM-dd"); //whatever format you need. 
    }

}
