using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal
{
    public class HistoricoTrocaAnexoSaidaModel
    {
        public int Id { get; set; }
        public string Numero { get; set; }
        public string Motivo { get; set; }
        public string NomeAutor { get; set; }
        public Guid GuidAutor { get; set; }
        public DateTime Alteracao { get; set; }

        public HistoricoTrocaAnexoSaidaModel DefinirNomeAutor(string nome)
        {
            this.NomeAutor= nome;
            return this;
        }

    }
}
