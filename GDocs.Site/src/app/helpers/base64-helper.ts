export class Base64Helper {

  static toArrayBuffer(base64: any): any {

    const binaryString = window.atob(base64);
    const len = binaryString.length;
    const bytes = new Uint8Array(len);
    for (let i = 0; i < len; i++) {
      bytes[i] = binaryString.charCodeAt(i);
    }
    return bytes;
  }

  static criarArquivoPdf(documento: string): string {
    const blob = new Blob([Base64Helper.toArrayBuffer(documento)], {
      type: 'application/pdf',
    });
    return URL.createObjectURL(blob);
  }

  static toBlob(base64: string, contentType: string) : Blob {
    return new Blob([Base64Helper.toArrayBuffer(base64)], {
      type: contentType,
    });
  }
}
