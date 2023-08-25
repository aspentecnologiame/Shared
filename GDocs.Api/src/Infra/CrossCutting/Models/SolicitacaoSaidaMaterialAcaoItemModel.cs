using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class SolicitacaoSaidaMaterialAcaoItemModel
    {
        public SolicitacaoSaidaMaterialAcaoItemModel(int idSolicitacaoSaidaMaterialItem)
        {
            IdSolicitacaoSaidaMaterialItem = idSolicitacaoSaidaMaterialItem;
        }

        public int Id { get; set; }
        public int IdSolicitacaoSaidaMaterialAcao { get; set; }
        public int IdSolicitacaoSaidaMaterialItem { get; set; }
        public bool FlgAtivo { get; set; } = true;
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }

        
    }
}
