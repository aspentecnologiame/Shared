using System;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class HistoricoProggoracaoModel
    {
        public int SolicitacaoMaterialItemId { get; set; }
        public DateTime DataRetornoAtual { get; set; }
        public DateTime DataRetornoNova { get; set; }
        public string Observacao { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }
        public string HtmlInfoDir { get; set; }
    }
}