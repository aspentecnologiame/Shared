using System;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class AssinaturaArmazenadaUsuarioModel
    {
        public Guid Guid { get; set; }
        public long BinarioId { get; set; }
        public byte[] AssinaturaArmazenadaBinario { get; set; }
        public long? BinarioAssinaturaDocumentoId { get; set; }
        public byte[] AssinaturaDocumentoArmazenadaBinario { get; set; }

        private string _assinaturaArmazenadaBase64;
        public string AssinaturaArmazenadaBase64
        {
            get
            {
                if (!string.IsNullOrEmpty(_assinaturaArmazenadaBase64))
                    return _assinaturaArmazenadaBase64;

                if (AssinaturaArmazenadaBinario != null)
                    return Convert.ToBase64String(AssinaturaArmazenadaBinario, Base64FormattingOptions.InsertLineBreaks);

                return null;
            }
            set { _assinaturaArmazenadaBase64 = value; }
        }
    }
}
