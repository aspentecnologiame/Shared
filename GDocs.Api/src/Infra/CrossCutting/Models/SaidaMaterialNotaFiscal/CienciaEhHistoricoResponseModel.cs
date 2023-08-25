using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal
{
    public class CienciaEhHistoricoResponseModel
    {
        public SaidaMaterialNotaFiscalCienciaModel SolicitacaoCiencia { get; set; }
        public IEnumerable<HistoricoProrrogacaoNotaFiscalModel> HistoricoProrrogacoes { get; set; }
        public IEnumerable<CienciaUsuarioAprovacaoModel> CienciaUsuarioAprovacao { get; set; }
        
    }
}
