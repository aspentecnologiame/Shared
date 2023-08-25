using System;
using System.Collections.Generic;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class RequisicaoUploadModel
    {
        public int CategoriaId { get; set; }
        public int? NumeroDocumento { get; set; }
        public IEnumerable<Guid> ListaGuidUsuarioAssinaturaDocumento { get; set; }
        public int QtdeDocumentos { get; set; }
    }
}
