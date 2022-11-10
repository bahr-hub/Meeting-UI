import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'itz-checkbox',
  templateUrl: './itz-checkbox.component.html',
  styleUrls: ['./itz-checkbox.component.scss']
})
export class ItzCheckboxComponent implements OnInit {

  @Input('checked') checked;
  @Input('disabled') disabled;
  @Output('_checked') _checked:EventEmitter<any> = new EventEmitter();

  constructor() { 
  }


 
 

  ngOnInit(): void {
  }

  checkTheBox()
  {
    if (this.disabled)
     return;
    this.checked = true;
    this._checked.emit();
  }

}
