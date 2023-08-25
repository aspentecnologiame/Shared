export class StringHelper {

  static parseToBoolean(value: string): boolean {
    return Boolean(JSON.parse(value));
  }
}
