import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'surround'
})
export class SurroundPipe implements PipeTransform {

  transform(value: any, start?: any, end?: any): any {
    if (!value) {
      return '';
    }

    if (!start) {
      start = '(';
    }

    if (!end) {
      end = ')';
    }

    return `${start}${value}${end}`;
  }

}
