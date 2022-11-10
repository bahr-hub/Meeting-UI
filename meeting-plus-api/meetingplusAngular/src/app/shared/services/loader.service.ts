import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoaderService {
  load: Subject<any>;
  finshed: Subject<any>;
  constructor() {
    this.load = new Subject<any>();
    this.finshed = new Subject<any>();
   }
   block()
   {
     this.load.next();
   }
   unblock()
   {
    this.finshed.next();
   }




}
