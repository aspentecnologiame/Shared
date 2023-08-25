import { NumeracaoAutomaticaModel } from "./numeracao-automatica.model";

export class AssinaturaInformacoesModel {
  public id: number = null;
  public titulo = '';
  public descricao = '';
  public nomeDocumento = '';
  public numeroDocumento: number = null;
  public listaNumeracaoAutomatica: NumeracaoAutomaticaModel[] = [];
  public destaque = false;
  public usuarioAd = '';
  public status: '';
  public dataCriacao: '';
  public categoriaId: number = null;
  public categoriaDescricao: '';
  public gerarNumeroDocumento = false;
  public numeroDocumentoAutomatico = false;
  public chaveNumeroDocumentoAutomatico = '';
  public certificadoDigital = false;
  public assinaturaDocumento = false;
  public assinadoNoDocumento = false;
}
