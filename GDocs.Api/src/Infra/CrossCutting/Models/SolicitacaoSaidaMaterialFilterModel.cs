using System;
using System.Collections.Generic;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class SolicitacaoSaidaMaterialFilterModel
    {
        public SolicitacaoSaidaMaterialFilterModel()
        {
            TiposSaida = new List<int>();
            Autores = new List<Guid>();
            Status = new List<int>();
        }

        public int Id { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataTermino { get; set; }
        public int Numero { get; set; }
        public IEnumerable<int> TiposSaida { get; set; }
        public string Patrimonio { get; set; }
        public IEnumerable<Guid> Autores { get; set; }
        public IEnumerable<int> Status { get; set; }
        public bool Vencido { get; set; }
        public Guid UsuarioLogadoAd { get; set; }
        public SolicitacaoSaidaMaterialOrdenacaoModel Ordenacao { get; set; }
    }
}
