import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'headerPipe'
})
export class HeaderPipePipe implements PipeTransform {

  transform(data: any[], filter?: any): any {
    return data.map(a => a[filter]);
  }
}

