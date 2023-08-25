import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'modelo-terminal'
})
export class ModeloTerminalPipe implements PipeTransform {

  transform(value: any, args?: any): any {
    return value.split('.', 2).join('.');
  }
}
