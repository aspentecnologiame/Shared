import { ItemSaidaMateialModel } from "./item-saida-material.model";

export class RegistroSaidaMaterial {

  public idSolicitacaoSaidaMaterial: number;
  public idSaidaMaterialTipoAcao: number;
  public solicitacaoMaterialAcaoItemId: number;

  public portador: string;
  public conferente: string;
  public dataAcao: string;
  public setorEmpresa: string;
  public observacao: string;

  public acaoItems: ItemSaidaMateialModel[] = [];
}
