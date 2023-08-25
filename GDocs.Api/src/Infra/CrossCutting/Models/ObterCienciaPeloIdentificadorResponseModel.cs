using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class ObterCienciaPeloIdentificadorResponseModel
    {
        public SolicitacaoCienciaModel SolicitacaoCiencia { get; set; }
        public IEnumerable<HistoricoProggoracaoModel> HistoricoProrrogacoes { get; set; }
        public IEnumerable<CienciaUsuarioAprovacaoModel> CienciaUsuarioAprovacao { get; set; }
    }
}