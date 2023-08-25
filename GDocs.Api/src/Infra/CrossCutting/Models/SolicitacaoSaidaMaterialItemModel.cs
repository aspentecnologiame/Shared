using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class SolicitacaoSaidaMaterialItemModel
    {
        public int IdSolicitacaoSaidaMaterialItem { get; set; }
        public int IdSolicitacaoSaidaMaterial { get; set; }
        public int IdSaidaMaterialTipoAcao { get; set; }
        public int Quantidade { get; set; }
        public string Unidade { get; set; }
        public string Patrimonio { get; set; }
        public string Descricao { get; set; }
        public string Status { get; set; }

        public DateTime DataCriacao { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }
}
