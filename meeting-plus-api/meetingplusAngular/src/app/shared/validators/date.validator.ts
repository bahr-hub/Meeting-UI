import { ValidatorFn, AbstractControl } from '@angular/forms';
import * as moment from "moment";

export function dateGreaterThan(date:any) :ValidatorFn{
    return( control: AbstractControl): { [key:string]: boolean } | null =>{
        //checking if the current date is smaller than the provided
        if(moment(control.value).isAfter(date))
        {
          
            return { validDate: true };
        }
        return { validDate: false };
    }
}

export function dateSmallerThan(date:any) :ValidatorFn{
    return( control: AbstractControl): { [key:string]: boolean } | null =>{
        if(moment(control.value).isBefore(date))
        {
            return { validDate: true };
        }
        return { validDate: false };
    }
}