import { Component, OnInit, Output,EventEmitter } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'itz-success-btn',
  templateUrl: './itz-success-btn.component.html',
  styleUrls: ['./itz-success-btn.component.scss']
})
export class ItzSuccessBtnComponent implements OnInit {

  @Output('clicked') clicked: EventEmitter<any> = new EventEmitter();
  constructor(public translate: TranslateService) { }

  ngOnInit(): void {
  }

}
