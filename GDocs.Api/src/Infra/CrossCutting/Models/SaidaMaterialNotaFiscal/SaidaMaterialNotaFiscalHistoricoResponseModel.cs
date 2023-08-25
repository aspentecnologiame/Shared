using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal
{
    public class SaidaMaterialNotaFiscalHistoricoResponseModel
    {
        public SaidaMaterialNotaFiscalModel SaidaMaterialNotaFiscal { get; set; }
        public IEnumerable<SaidaMaterialNotaFiscalAcaoModel> SaidaMaterialNotaFiscalAcoes { get; set; }
        public IEnumerable<HistoricoProrrogacaoNotaFiscalModel> HistoricoProrrogacoes { get; set; }
        public IEnumerable<HistoricoProrrogacaoNotaFiscalModel> HistoricoBaixaSemRetorno { get; set; }
        public IEnumerable<HistoricoTrocaAnexoSaidaModel> HistoricoTrocaAnexoSaida { get; set; }
        
    }
}
