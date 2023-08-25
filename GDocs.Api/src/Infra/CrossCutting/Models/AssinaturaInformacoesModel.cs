using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using System;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class AssinaturaInformacoesModel
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string NomeDocumento { get; set; }
        public int? NumeroDocumento { get; set; }
        public bool Destaque { get; set; }
        public string UsuarioAd { get; set; }
        public Guid UsuarioGuidAd { get; set; }
        public StatusAssinaturaDocumento Status { get; set; }
        public string StatusDescricao { get; set; }
        public DateTime DataCriacao { get; set; }
        public string DataCriacaoFormatada { get; set; }
        public int CategoriaId { get; set; }
        public string CategoriaDescricao { get; set; }
        public bool GerarNumeroDocumento { get; set; }
        public bool NumeroDocumentoAutomatico { get; set; }
        public string ChaveNumeroDocumentoAutomatico { get; set; }
        public bool CertificadoDigital { get; set; }
        public bool AssinadoNoDocumento { get; set; }

        public AssinaturaInformacoesModel DefinirAutor(string autor)
        {
            UsuarioAd = autor;
            return this;
        }

        public AssinaturaInformacoesModel DefinirStatus(StatusAssinaturaDocumento status)
        {
            Status = status;
            return this;
        }
    }
}
