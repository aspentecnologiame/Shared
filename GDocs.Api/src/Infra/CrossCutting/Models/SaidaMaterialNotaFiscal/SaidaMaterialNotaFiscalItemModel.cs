using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal
{
    public class SaidaMaterialNotaFiscalItemModel
    {
        public int Id { get; set; }
        public int IdSolicitacaoSaidaMaterialNF { get; set; }
        public int IdSaidaMaterialNotaFiscalItem { get; set; }
        public int Quantidade { get; set; }
        public string Unidade { get; set; }
        public string Codigo { get; set; }
        public double ValorUnitario { get; set; }
        public string TagService { get; set; }
        public string Patrimonio { get; set; }
        public string Descricao { get; set; }
        public string Status { get; set; }
        public DateTime DataCriacao { get; set; }
        public bool Ativo { get; set; } = true;
        public DateTime DataAtualizacao { get; set; }
    }
}


 