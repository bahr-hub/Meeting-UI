import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ItzSuccessBtnComponent } from './itz-success-btn/itz-success-btn.component';



@NgModule({
  declarations: [ItzSuccessBtnComponent],
  imports: [
    CommonModule
  ],
  exports:
  [
    ItzSuccessBtnComponent
  ]
})
export class ItzButtonsModule { }
