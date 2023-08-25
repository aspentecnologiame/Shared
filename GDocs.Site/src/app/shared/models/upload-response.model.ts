export class UploadResponseModel {
  constructor(public nomeOriginal: string = '',
    public nomeSalvo: string = '',
    public arquivoBinario: string = '',
    public pdfCaminhoArquivo: string = '',
    public numeracaoAutomatica?: number,
    public selecionado: boolean = false
  ) { }
}
