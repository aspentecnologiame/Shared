import { HistoricoTrocaAnexoSaidaModel } from "./Historico-troca-anexo-saida-model";
import { HistoricoProrrogacaoNotaFiscalModel } from "./HistoricoProrrogacaoNotaFiscalModel";
import { SaidaMaterialNotaFiscal } from "./SaidaMaterialNotaFiscal";
import { SaidaMaterialNotafiscalAcaoModel } from "./SaidaMaterialNotafiscalAcaoModel";

export class HistoricoNotaFiscalResponseModel{
    public saidaMaterialNotaFiscal: SaidaMaterialNotaFiscal;
    public historicoBaixaSemRetorno:HistoricoProrrogacaoNotaFiscalModel;
    public solicitacaoMaterialAcoes:SaidaMaterialNotafiscalAcaoModel;
    public historicoProrogacoes:HistoricoProrrogacaoNotaFiscalModel;
    public historicoTrocaAnexoSaida:HistoricoTrocaAnexoSaidaModel;
}
