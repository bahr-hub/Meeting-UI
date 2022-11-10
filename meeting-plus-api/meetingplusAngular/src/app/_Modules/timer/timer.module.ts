import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TimerComponent } from './timer/timer.component';
import { SharedModule } from 'src/app/shared/shared.module';



@NgModule({
  declarations: [TimerComponent],
  imports: [
    CommonModule,
    SharedModule.forRoot()
  ],
  exports: [TimerComponent]
})
export class TimerModule { }
