using System;
using System.Collections.Generic;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class DocumentoFI1548FilterModel
    {
        public DocumentoFI1548FilterModel()
        {
            TiposPagamento = new List<string>();
            AutoresADId = new List<Guid>();
            Status = new List<int>();
        }
        public int Id { get; set; }
        public int Numero { get; set; }
        public IEnumerable<string> TiposPagamento { get; set; }
        public IEnumerable<Guid> AutoresADId { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataTermino { get; set; }
        public IEnumerable<int> Status { get; set; }
        public Guid UsuarioLogadoAd { get; set; }
        public DocumentoFI1548OrdenacaoModel Ordenar { get; set; }
    }
}
