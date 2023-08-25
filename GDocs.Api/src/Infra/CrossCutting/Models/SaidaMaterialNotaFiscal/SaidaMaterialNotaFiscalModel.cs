using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal
{
    public class SaidaMaterialNotaFiscalModel
    {
        public int Id { get; set; }
        public int? Numero { get; set; }
        public string TipoSaida { get; set; }
        public bool FlgRetorno { get; set; }
        public Guid GuidAutor { get; set; }
        public Guid GuidResponsavel { get; set; }
        public string NomeResponsavel { get; set; }
        public string NomeAutor { get; set; }
        public string EmailAutor { get; set; }
        public string SetorResponsavel { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public DateTime? Retorno { get; set; }
        public DateTime Saida { get; set; }
        public DateTime? DataAcao { get; set; }
        public string DataCriacaoFormatada { get; set; }
        public string DataRetornoFormatada { get; set; }
        public string DataSaidaFormatada { get; set; }
        public string Motivo { get; set; }
        public bool Ativo { get; set; }
        public long BinarioId { get; set; }
        public int StatusId { get; set; }
        public string Status { get; set; }
        public string Documento { get; set; }
        public string Volume { get; set; }
        public string Peso { get; set; }
        public string Transportador { get; set; }
        public string CodigoTotvs { get; set; }
        public int ModalidadeFrete { get; set; }
        public string ModalidadeFreteDesc { get; set; }
        public int NaturezaOperacional { get; set; }
        public string NaturezaOperacionalDesc { get; set; }
        public Fornecedor Fornecedor { get; set; }
        public string HtmlItens { get; set; }
        public DateTime DataCriacao { get; set; }
        public bool RetornoParcial {get; set;}

        public List<SaidaMaterialNotaFiscalItemModel> ItemMaterialNf => _itemMaterialNf;
        private readonly List<SaidaMaterialNotaFiscalItemModel> _itemMaterialNf;

        public SaidaMaterialNotaFiscalModel()
        {
            _itemMaterialNf = new List<SaidaMaterialNotaFiscalItemModel>();
        }

        public SaidaMaterialNotaFiscalModel DefinirID(int id)
        {
            Id= id;
            return this;
        }

        public SaidaMaterialNotaFiscalModel DefinirNomeAutor(string nome)
        {
            NomeAutor = nome;
            return this;
        }
        public SaidaMaterialNotaFiscalModel DefinirEmailAutor(string email)
        {
            EmailAutor = email;
            return this;
        }

        public SaidaMaterialNotaFiscalModel DefinirNomeResponsavel(string nomeResponsavel)
        {
            NomeResponsavel = nomeResponsavel;
            return this;
        }
    }
}

 