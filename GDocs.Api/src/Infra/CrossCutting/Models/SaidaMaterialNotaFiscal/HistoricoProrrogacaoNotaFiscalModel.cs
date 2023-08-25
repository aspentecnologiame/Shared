using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal
{
    public class HistoricoProrrogacaoNotaFiscalModel
    {
        public int SaidaMaterialNotaFiscalItemId { get; set; }
        public DateTime DataRetornoAtual { get; set; }
        public DateTime DataRetornoNova { get; set; }
        public string Observacao { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }
        public string HtmlInfoDir { get; set; }
    }
}
