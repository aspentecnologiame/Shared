using System;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class AssinaturaPassoAssinanteRepresentanteModel
    {
        public int PassoId { get; set; }
        public Guid UsuarioAdAssinanteGuid { get; set; }
        public string UsuarioAdAssinate { get; set; }
        public Guid? UsuarioAdRepresentanteGuid { get; set; }
        public string UsuarioAdRepresentante { get; set; }
        public bool PodeSerRepresentado { get; set; }
        public bool FareiEnvio { get; set; }

        public AssinaturaPassoAssinanteRepresentanteModel()
        {
            PodeSerRepresentado = true;
        }

        public AssinaturaPassoAssinanteRepresentanteModel DefinirAssinante(string usuarioAdAssinate)
        {
            UsuarioAdAssinate = usuarioAdAssinate;
            return this;
        }
    }
}
