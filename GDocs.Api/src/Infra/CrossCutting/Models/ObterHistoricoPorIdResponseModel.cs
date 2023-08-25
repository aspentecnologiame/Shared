using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class HistoricoResponseModel
    {
        public SolicitacaoSaidaMaterialModel SolicitacaoMaterial { get; set; }
        public IEnumerable<SolicitacaoSaidaMaterialAcaoModel> SolicitacaoMaterialAcao { get; set; }
        public IEnumerable<HistoricoProggoracaoModel> HistoricoProrogacao { get; set; }
        public IEnumerable<HistoricoProggoracaoModel> HistoricoBaixaSemRetorno { get; set; }

    }
}
