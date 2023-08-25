import { ItemSaidaMaterialNF } from "./ItemSaidaMaterialNF";

export class SaidaMaterialNotaFiscalCienciaModel {
  public id: number;
  public idSolicitacaoSaidaMaterial: number;
  public IdSolicitacaoSaidaMaterialAcao: number;
  public idTipoCiencia: number;
  public cienciaId: number;
  public tipoCiencia: string;
  public idStatusCiencia: number;
  public statusCiencia: string;
  public dataProrrogacaoNF: string;
  public dataRetorno:string;
  public idUsuario: string;
  public nomeUsuario: string;
  public observacao: string;
  public flgAtivo: boolean;
  public numeroMaterial: number;
  public motivoSolicitacao: string;
  public justificativa: string;
  public ciente: boolean;

 public itens:ItemSaidaMaterialNF[] = [];
}
