export class UploadRequestModel {
  constructor(
    public file: File,
    public categoriaId: number,
    public guidUsuarioAssinaturaDocumento: any[],
    public qtdeDocumentos: number,
    public numeroDocumento?: number
  ) { }
}
