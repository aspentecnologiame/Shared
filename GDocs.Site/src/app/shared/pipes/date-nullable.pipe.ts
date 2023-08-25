import { Pipe, PipeTransform } from '@angular/core';
import { DatePipe } from '@angular/common';

@Pipe({
    name: 'dateNullable'
})
export class DateNullablePipe extends DatePipe implements PipeTransform {
    transform(value: any, args?: any): any {
        if (isNaN(value)) {
            return '';
        }
        return super.transform(value, args);
    }
}
