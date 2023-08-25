using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class SolicitacaoSaidaMaterialAcaoTipoModel
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public bool FlgAtivo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }
}
