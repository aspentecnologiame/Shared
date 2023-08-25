using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class ObterMotivoCancelamentoModel
    {
        public int Id { get; set; }
        public Guid UsuarioGuid { get; set; }
        public string Nome { get; set; }
        public string Observacao { get; set; }
        public DateTime DataCancelamento { get; set; }
        public string TipoCancelamento { get; set; }
    }
}