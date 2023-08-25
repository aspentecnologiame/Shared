import { NgModule, Injectable } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
@NgModule({
  imports: [
    CommonModule
  ],
  declarations: []
})
export class MatGroupBy<T> {

  public groupingChange: BehaviorSubject<Grouping<T>>;

  constructor() {
    this.groupingChange = new BehaviorSubject<Grouping<T>>(this.grouping);
  }

  public get grouping(): Grouping<T> { return this._grouping; }
  public set grouping(grouping: Grouping<T>) {
    this._grouping = grouping;
    this.groupingChange.next(this.grouping);
  }
  private _grouping: Grouping<T>;

  public isGroup(index, item): boolean {
    return item.level;
  }

  public toggleExpanded(row) {
    row.expanded = !row.expanded;
    this.groupingChange.next(this.grouping);
  }

  public groupData(data: T[]): (T | Group)[] {
    if (!this._grouping.groupingDataAccessor) {
      return data;
    }

    let correntgroup: Group = new Group();

    const dataResult: (T | Group)[] = [];

    data.forEach(row => {

      const tempGroupAcessor = this._grouping.groupingDataAccessor(row);

      if (correntgroup.value !== tempGroupAcessor) {

        correntgroup = new Group();
        correntgroup.level = 1;
        correntgroup.value = tempGroupAcessor;

        dataResult.push(correntgroup);
      }

      dataResult.push(row);

    });

    return dataResult;
  }
}

export class Grouping<T> {
  constructor() {
    /* Only comments */
  }

  groupingDataAccessor: ((row: T) => string);
}

export class Group {
  level = 0;
  name: string;
  value: any = '';
  expanded = true;
}
