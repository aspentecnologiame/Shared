import { CienciaUsuarioAprovacaoModel } from "./ciencia-usuario-aprovacao-model";
import { HistoricoProrrogacaoNotaFiscalModel } from "./HistoricoProrrogacaoNotaFiscalModel";
import { SaidaMaterialNotaFiscalCienciaModel } from "./saida-material-nota-fiscal-ciencia-model";

export class CienciaEhHistoricoResponseModel{
  public solicitacaoCiencia: SaidaMaterialNotaFiscalCienciaModel;
  public historicoProrrogacoes:HistoricoProrrogacaoNotaFiscalModel;
  public cienciaUsuarioAprovacao:CienciaUsuarioAprovacaoModel[];

}
