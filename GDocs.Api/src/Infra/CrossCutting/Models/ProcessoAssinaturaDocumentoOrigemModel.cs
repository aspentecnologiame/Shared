using System;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class ProcessoAssinaturaDocumentoOrigemModel
    {
        public int ProcessoAssinaturaDocumentoId { get; set; }
        public string ProcessoAssinaturaDocumentoOrigemId { get; set; }
        public string ProcessoAssinaturaDocumentoOrigemNome { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public bool Ativo { get; set; }
    }
}
