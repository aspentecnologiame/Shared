using System;
using System.Collections.Generic;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class SolicitacaoCienciaModel
    {
        public int Id { get; set; }
        public int IdSolicitacaoSaidaMaterial { get; set; }
        public int IdSolicitacaoSaidaMaterialAcao { get; set; }
        public int IdTipoCiencia { get; set; }
        public string TipoCiencia { get; set; }
        public int IdStatusCiencia { get; set; }
        public string StatusCiencia { get; set; }
        public DateTime? DataProrrogacao { get; set; } = DateTime.Now;
        public DateTime DataRetorno { get; set; } 
        public Guid IdUsuario { get; set; }
        public string NomeUsuario { get; set; }
        public string Observacao { get; set; }
        public bool FlgAtivo { get; set; }
        public bool Ciente { get; set; } = false;
        public int NumeroMaterial { get; set; }
        public string MotivoSolicitacao { get; set; }
        public string Justificativa { get; set; }
        public List<SolicitacaoCienciaItemModel> Itens => _itens;

        private readonly List<SolicitacaoCienciaItemModel> _itens;
        public SolicitacaoCienciaModel()
        {
            _itens = new List<SolicitacaoCienciaItemModel>();
        }

        public SolicitacaoCienciaModel DefinirNomeUsuario(string nomeUsuario)
        {
            NomeUsuario = nomeUsuario;
            return this;
        }

    }
}