using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using Newtonsoft.Json.Linq;
using System;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class DocumentoFI1548Model 
    {
        public DocumentoFI1548Model()
        {
            Status = DocumentoFI1548Status.EmConstrucao;
            Ativo = true;
        }

        public int Id { get; set; }
        public string Descricao { get; set; }
        public string Referencia { get; set; }
        public decimal Valor { get; set; }
        public string TipoPagamento { get; set; }
        public int QuantidadeParcelas { get; set; }
        public DocumentoFI1548Status Status { get; set; }
        public string DescricaoStatus { get; set; }
        public Guid AutorId { get; set; }
        public string Autor { get; set; }
        public string NomeSalvo { get; set; }
        public long? BinarioId { get; set; }
        public bool Ativo { get; set; }
        public DateTime? DataLiquidacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public byte[] ArquivoBinario { get; set; }
        public int Numero { get; set; }
        public string MoedaSimbolo { get; set; }
        public string Fornecedor { get; set; }
        public int VencimentoPara { get; set; }
        public string PrazoEntrega { get; set; }
        public bool Destaque { get; set; }
        public int? ReferenciaSubstituto { get; set; }

        public DocumentoFI1548Model DefinirId(int id)
        {
            Id = id;
            return this;
        }

        public DocumentoFI1548Model DefinirAutor(string nomeAutor)
        {
            Autor = nomeAutor;
            return this;
        }

        public DocumentoFI1548Model DefinirAutor(Guid idAutor, string nomeAutor)
        {
            AutorId = idAutor;
            DefinirAutor(nomeAutor);
            return this;
        }

        public DocumentoFI1548Model DefinirComoLiquidado()
        {
            Status = DocumentoFI1548Status.Liquidado;
            DataLiquidacao = DateTime.Now;

            return this;
        }

        public DocumentoFI1548Model DefinirComoCancelado()
        {
            Status = DocumentoFI1548Status.Cancelado;
            return this;
        }

        public DocumentoFI1548Model DefinirNomeArquivoSalvo(string nomeSalvo)
        {
            NomeSalvo = nomeSalvo;
            return this;
        }

        public DocumentoFI1548Model DefinirBinario(long id)
        {
            BinarioId = id;
            return this;
        }

        public DocumentoFI1548Model DefinirNumero(int numero)
        {
            Numero = numero;
            return this;
        }
        public bool PodeEditar()
        {
            return this.Id != 0;
        }

        public bool ExisteBinario()
        {
            return  BinarioId != 0 && BinarioId != null;
        }


    }
}
