import { Pipe, PipeTransform } from '@angular/core';


@Pipe({
  name: 'cpfcnpj'
})
export class CpfCnpjPipe implements PipeTransform {

  transform(value: string, args?: any): any {
    if (!value && value.length < 10) {
      return value;
    }
    let result = value.toString();

    if(result.length < 12){
      return `${result.substring(0,3)}.${result.substring(6,3)}.${result.substring(9,6)}-${result.substring(12,9)}`
    }else{
      return  `${result.substring(0,2)}.${result.substring(2, 5)}.${result.substring(5, 8)}/${result.substring(8, 12)}-${result.substring(12)}`
    }
  }
}
