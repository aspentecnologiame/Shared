import { isNumber } from 'util';
import { Observable } from 'rxjs';
import { scan, delay, tap } from 'rxjs/operators';

export const genericRetryStrategy = ({
  maxRetryAttempts = 3,
  delayMs = 2000,
  excludedStatusCodes = []
}: {
  maxRetryAttempts?: number,
  delayMs?: number,
  excludedStatusCodes?: number[]
} = {}) => (errors: Observable<any>) => {

  return errors.pipe(
    tap(err => {
      if (excludedStatusCodes.find(e => e === err.status)) {
        throw err;
      }
    }),
    delay(delayMs),
    scan((errorCount, err) => {
      const attempt = isNumber(errorCount) ? errorCount + 1 : 1;

      if (errorCount >= maxRetryAttempts || excludedStatusCodes.find(e => e === err.status)) {
        throw err;
      }
      return attempt;
    })
  );
};
