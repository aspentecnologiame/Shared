
import { Fornecedor } from "./fornecedor";
import { ItemSaidaMaterialNF } from "./ItemSaidaMaterialNF";

export class SaidaMaterialNotaFiscal {

  public id: number;
  public numero?: number;
  public flgRetorno: boolean;
  public setorResponsavel: string;
  public nomeAutor: string;
  public guidAutor: string;
  public origem: string;
  public destino: string;
  public retorno: Date;
  public dataAcao: Date;
  public saida: Date;
  public motivo: string;
  public status: string;
  public statusId: number;
  public tipoSaida: string;
  public documento: number;
  public volume: string;
  public peso: string;
  public transportador: string;
  public codigoTotvs: string;
  public modalidadeFrete: number;
  public modalidadeFreteDesc: string;
  public naturezaOperacional: number;
  public naturezaOperacionalDesc: string;
  public fornecedor:Fornecedor;
  public itemMaterialNf: ItemSaidaMaterialNF[] = [];
  public htmlItens: string;
  public retornoParcial: boolean;
}
