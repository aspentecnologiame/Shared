using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using System;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class ProcessoAssinaturaDocumentoModel
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string NomeDocumento { get; set; }
        public bool Destaque { get; set; }
        public Guid AutorId { get; set; }
        public int StatusId { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? Numero { get; set; }
        public int CategoriaId { get; set; }
        public string DescricaoCategoria { get; set; }
        public StatusAssinaturaDocumentoPassoAtual StatusPassoAtualId { get; set; }
        public bool AssinarCertificadoDigital { get; set; }
        public bool AssinarFisicamente { get; set; }
        public bool UsuarioDaConsultaAssinou { get; set; }
        public bool AssinadoPorMimVisivel { get; set; }
        public bool ReferenciaSubstituto { get; set; }
        public int? CienciaId { get; set; }
        public string Observacao { get; set; }
    }
}
