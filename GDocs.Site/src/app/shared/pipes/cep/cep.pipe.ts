import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'cep'
})
export class CepPipe implements PipeTransform {

  transform(value: any, args?: any): any {

    if (!value && value.length < 8) {
      return value;
    }
    let result = value.toString();
    return `${result.substring(0, 5)}-${result.substring(5)}`
  }
}




