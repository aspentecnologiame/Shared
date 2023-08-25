import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'numeroDecimal'
})
export class NumeroDecimalPipe implements PipeTransform {

  transform(value: any, args?: any): any {
    return value.toFixed(2).toString().replace('.', ',');
  }
}
