using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal
{
    public class SaidaMaterialNotaFiscalCienciaItemModel
    {
        public int IdItem { get; set; }
        public int IdSaidaMaterialNotaFiscalItem { get; set; }
        public int Quantidade { get; set; }
        public string Unidade { get; set; }
        public string Patrimonio { get; set; }
        public string Descricao { get; set; }
        public string Codigo { get; set; }
        public double ValorUnitario { get; set; }
        public string TagService { get; set; }
    }
}
