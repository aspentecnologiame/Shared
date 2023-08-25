using System;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class AssinaturaArquivoModel
    {
        public object Id { get; set; }
        public int Ordem { get; set; }
        public long BinarioId { get; set; }
        public byte[] ArquivoBinario { get; set; }
        public string NomeSalvo { get; set; }
        public int ProcessoAssinaturaDocumentoId { get; set; }
        public int ProcessoAssinaturaDocumentoStatusId { get; set; }
        public Guid AutorId { get; set; }
        public string Autor { get; set; }
        public bool ArquivoFinal { get; set; }
        public bool Status { get; set; }
        public DateTime Atualizacao { get; set; }
        public int Categoria { get; set; }

        public AssinaturaArquivoModel DefinirAutor(string nomeAutor)
        {
            Autor = nomeAutor;
            return this;
        }
    }
}
