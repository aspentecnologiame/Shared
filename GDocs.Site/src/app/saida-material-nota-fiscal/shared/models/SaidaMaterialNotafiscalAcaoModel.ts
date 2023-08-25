import { ItemSaidaMaterialNF } from "./ItemSaidaMaterialNF";

export class SaidaMaterialNotafiscalAcaoModel {

  public idSaidaMaterialNotaFiscal: number;
  public idSaidaMaterialNotaFiscalTipoAcao: number;
  public idSaidaMaterialNotaFiscalAcaoItem: number;

  public portador: string;
  public conferente: string;
  public dataAcao: string;
  public setorEmpresa: string;
  public observacao: string;

  public acaoItems: ItemSaidaMaterialNF[] = [];
}
