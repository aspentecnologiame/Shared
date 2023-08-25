using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal
{
    public class CienciaUsuarioAprovacaoModel
    {
        public int Id { get; set; }
        public int SolicitacaoCienciaId { get; set; }
        public Guid UsuarioGuid { get; set; }
        public string Nome { get; set; }
        public string Observacao { get; set; }
        public DateTime DataAprovacao { get; set; }
        public bool FlgRejeitado { get; set; }

        public CienciaUsuarioAprovacaoModel DefinirNome(string nome) 
        {
            Nome = nome;
            return this;
        }

    }
}
