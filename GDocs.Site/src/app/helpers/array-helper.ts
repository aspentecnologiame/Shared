import { uniq, filter, isEmpty } from 'lodash';

export class ArrayHelper {

    static unique<T>(itens: Array<T>): Array<T> {
        return uniq(itens);
    }

    static mapAndUnique<T, U>(itens: Array<T>, callbackfn: (value: T, index: number, array: T[]) => U, thisArg?: any): Array<U> {
        return uniq(itens.map(callbackfn));
    }

    static filterTree<T>(
      predicate: (node: T) => boolean,
      list: T[],
      childrenField: string
    ) {
      return filter(list, (item) => {
        if (predicate(item)) {
          return true;
        } else if (item[childrenField]) {
          item[childrenField] = ArrayHelper.filterTree(predicate, item[childrenField], childrenField);
          return !isEmpty(item[childrenField]);
        }

        return false;
      });
    }
}
