export class AssinaturaUploadResponseModel {
  public id: number;
  public binarioId: number;
  public processoAssinaturaDocumentoId: number;
  public processoAssinaturaDocumentoStatusId: number;
  public ordem: number;

  public nomeOriginal: string;
  public nomeSalvo: string;
  public arquivoBinario: string;

  public pdfCaminhoArquivo: string;
  public selecionado: boolean;
  public arquivoFinal: boolean;
  public status: boolean;
  public atualizacao: number;


  public autorId: string;
  public autor: string;

  public categoria: number;

}
