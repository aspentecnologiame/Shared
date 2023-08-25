using ICE.GDocs.Common.Core.Domain.ValueObjects;
using System;

namespace ICE.GDocs.Domain.Models.Trace
{
    public class DadosRequisicao
    {
        public string Acao { get; private set; }

        public Metadados Metadados { get; private set; }

        public Metadados Conteudo { get; private set; }        

        private DadosRequisicao()
        {
        }

        private DadosRequisicao(string acao, Metadados metadados, Metadados conteudo)
            : this()
        {
            Acao = acao;
            Metadados = metadados;
            Conteudo = conteudo;            
        }

        public static DadosRequisicao Criar(string acao, Metadados metadados, Metadados conteudo)
            => new DadosRequisicao(acao, metadados, conteudo);
    }
}
