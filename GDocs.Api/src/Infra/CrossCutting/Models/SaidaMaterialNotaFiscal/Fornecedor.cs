using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal
{
    public class Fornecedor
    {
        public int Id { get; set; }
        public int SolicitacaoMaterialNfId { get; set; }
        public string Endereco { get; set; }
        public string Bairro { get; set; }
        public string Cep { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public bool Ativo { get; set; } = true;

        public Fornecedor DefinirSolicitacaoMaterialNfId(int id) { 
            SolicitacaoMaterialNfId = id;
            return this;
        }

        public Fornecedor DefinirId(int id) 
        {
            Id = id;
            return this;
        }

        public bool ExisteFornecedor()
        { 
           return this.Id != 0;
        }
    }
}
