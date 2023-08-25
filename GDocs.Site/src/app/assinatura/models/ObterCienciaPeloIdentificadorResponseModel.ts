import { HistorioProrrogacaoModel } from "src/app/FI347/shared/models/historico-prorrogacao.model";
import { SolicitacaoCienciaModel } from "./solicitacao-ciencia-model";

export class ObterCienciaPeloIdentificadorResponseModel{
  public solicitacaoCiencia: SolicitacaoCienciaModel;
  public historicoProrrogacoes:HistorioProrrogacaoModel;
}
