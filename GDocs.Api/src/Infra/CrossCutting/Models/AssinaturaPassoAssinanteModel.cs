using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using System;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class AssinaturaPassoAssinanteModel
    {
        public int ProcessoAssinaturaId { get; set; }
        public int ProcessoAssinaturaDocumentoId { get; set; }
        public Guid UsuarioAdAssinanteGuid { get; set; }
        public Guid? UsuarioAdRepresentanteGuid { get; set; }
        public StatusAssinaturaDocumentoPassoUsuario Status { get; set; }
        public bool NotificarFinalizacao { get; set; }
        public DateTime DataAssinatura { get; set; }
        public string Justificativa { get; set; }
        public bool AssinarCertificadoDigital { get; set; }
        public bool AssinarFisicamente { get; set; }
        public long ProcessoAssinaturaArquivoCorrenteId { get; set; }
        public long BinarioId { get; set; }
    }
}
