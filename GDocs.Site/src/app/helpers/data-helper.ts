export class DataHelper {

  static parseToPortugueseStringDate(data: Date): string {
    if (data === null) {
      return '';
    }
    const dia = data.getDate().toString().padStart(2, '0');
    const mes = (data.getMonth() + 1).toString().padStart(2, '0');
    const ano = data.getFullYear();
    return `${dia}/${mes}/${ano}`;
  }

  static parseToPortugueseStringDateHour(data: Date): string {
    if (data === null) {
      return '';
    }
    const dia = data.getDate().toString().padStart(2, '0');
    const mes = (data.getMonth() + 1).toString().padStart(2, '0');
    const ano = data.getFullYear();
    const hora = data.getHours().toString().padStart(2, '0');
    const minuto = data.getMinutes().toString().padStart(2, '0');

    return `${dia}/${mes}/${ano} ${hora}:${minuto}`;
  }

  static addSpecificTimeToDateTime(data: Date, hora: number, minuto: number): Date {
    if (data === null) {
      return data;
    }

    const dia = Number(data.getDate().toString().padStart(2, '0'));
    const mes = Number((data.getMonth() + 1).toString().padStart(2, '0'));
    const ano = Number(data.getFullYear());


    return new Date(ano, mes, dia, hora, minuto);
  }
}
