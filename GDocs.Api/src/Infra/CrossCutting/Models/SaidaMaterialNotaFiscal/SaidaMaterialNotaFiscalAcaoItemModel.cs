using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal
{
    public class SaidaMaterialNotaFiscalAcaoItemModel
    {
        public SaidaMaterialNotaFiscalAcaoItemModel(int idSaidaMaterialNotaFiscalItem)
        {
            IdSaidaMaterialNotaFiscalItem = idSaidaMaterialNotaFiscalItem;
        }

        public int Id { get; set; }
        public int IdSolicitacaoSaidaMaterialNF { get; set; }
        public int IdSaidaMaterialNotaFiscalAcao { get; set; }
        public int IdSaidaMaterialNotaFiscalItem { get; set; }
        public bool FlgAtivo { get; set; } = true;
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }
}
