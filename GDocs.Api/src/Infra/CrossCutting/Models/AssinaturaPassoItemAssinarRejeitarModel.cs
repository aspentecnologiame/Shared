using System.Collections.Generic;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class AssinaturaPassoItemAssinarRejeitarModel
    {
        public IEnumerable<int> ListaDeprocessoAssinaturaDocumentoId { get; set; }

        public string Justificativa { get; set; }
        public UploadModel UploadDocumentoComAssinaturaDigital { get; set; }
    }
}
