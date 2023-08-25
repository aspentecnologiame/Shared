import { ItemSaidaMateialModel } from "./item-saida-material.model";

export class SaidaMaterialModel {

  public id: number;
  public numero: string;
  public flgRetorno: boolean;
  public setorResponsavel: string;
  public guidResponsavel: string;
  public autor: string;
  public origem: string;
  public destino: string;
  public retorno: Date;
  public dataAcao: Date;
  public motivo: string;
  public observacao: string;
  public status: string;
  public statusId: number;
  public tipoSaida: string;
  public nomeResponsavel: string;
  public htmlItens: string;
  public retornoParcial: boolean;
  public binario: string;

  public itemMaterial: ItemSaidaMateialModel[] = [];
}
