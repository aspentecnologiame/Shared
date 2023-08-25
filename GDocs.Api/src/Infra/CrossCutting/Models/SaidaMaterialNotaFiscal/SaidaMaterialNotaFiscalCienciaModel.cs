using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal
{
    public class SaidaMaterialNotaFiscalCienciaModel
    {
        public int Id { get; set; }
        public int IdSaidaMaterialNotaFiscal { get; set; }
        public int IdSaidaMaterialNotaFiscalAcao { get; set; }
        public int IdTipoCiencia { get; set; }
        public string TipoCiencia { get; set; }
        public int IdStatusCiencia { get; set; }
        public string StatusCiencia { get; set; }
        public DateTime? DataProrrogacaoNF { get; set; } = DateTime.Now;
        public DateTime DataRetorno { get; set; }
        public Guid IdUsuario { get; set; }
        public string NomeDeUsuario { get; set; }
        public string Observacao { get; set; }
        public bool FlgAtivo { get; set; }
        public bool Ciente { get; set; } = false;
        public int NumeroMaterial { get; set; }
        public string MotivoSolicitacao { get; set; }
        public string Justificativa { get; set; }


        public List<SaidaMaterialNotaFiscalCienciaItemModel> Itens => _itens;

        private readonly List<SaidaMaterialNotaFiscalCienciaItemModel> _itens;

        public SaidaMaterialNotaFiscalCienciaModel()
        {
            _itens = new List<SaidaMaterialNotaFiscalCienciaItemModel>();
        }

        public SaidaMaterialNotaFiscalCienciaModel DefinirNomeUsuario(string nomeDeUsuario)
        {
            NomeDeUsuario = nomeDeUsuario;
            return this;
        }
    }
}
