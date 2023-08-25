import { HistorioProrrogacaoModel } from "./historico-prorrogacao.model";
import { RegistroSaidaMaterial } from "./registro-saida-material";
import { SaidaMaterialModel } from "./saida-material.model";

export class HistoricoResponseModel{
  public solicitacaoMaterial: SaidaMaterialModel;
  public historicoBaixaSemRetorno:HistorioProrrogacaoModel;
  public solicitacaoMaterialAcao:RegistroSaidaMaterial;
  public historicoProrogacao:HistorioProrrogacaoModel;
}
