using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using System;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class AssinaturaUsuarioModel
    {
        public int Id { get; set; }
        public int PassoId { get; set; }
        public Guid Guid { get; set; }
        public string Nome { get; set; }
        public bool NotificarFinalizacao { get; set; }
        public string FluxoNotificacao { get; set; }
        public StatusAssinaturaDocumentoPassoUsuario Status { get; set; }
        public string DescricaoStatus { get; set; }
        public DateTime? DataAssinatura { get; set; }
        public Guid GuidRepresentante { get; set; }
        public string NomeRepresentante { get; set; }
        public string Justificativa { get; set; }
        public bool AssinarDigitalmente { get; set; }
        public bool AssinarFisicamente { get; set; }
        public bool FlagFareiEnvio { get; set; }


        public AssinaturaUsuarioModel()
        {
            Status = StatusAssinaturaDocumentoPassoUsuario.NaoIniciado;
            FluxoNotificacao = "Padrão";
        }

        public AssinaturaUsuarioModel DefinirNome(string nome)
        {
            Nome = nome;
            return this;
        }
    }
}
