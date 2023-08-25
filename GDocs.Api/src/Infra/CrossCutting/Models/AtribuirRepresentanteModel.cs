using System.Collections.Generic;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class AtribuirRepresentanteModel
    {
        public IEnumerable<AssinaturaPassoAssinanteRepresentanteModel> Assinantes { get; set; }
    }
}
