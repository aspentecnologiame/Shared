using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal
{
    public class SaidaMaterialArquivoModel
    {
        public object Id { get; set; }
        public int SaidaMaterialNfId { get; set; }
        public int Numero { get; set; }
        public long BinarioId { get; set; }
        public byte[] ArquivoBinario { get; set; }
        public string Arquivo { get; set; }
        public Guid GuidAutor { get; set; }
        public string Motivo { get; set; }
        public string NomeSalvo { get; set; }
        public string NomeOriginal { get; set; }
        public int TipoNatureza { get; set; }
        public bool Ativo { get; set; } = true;
        public DateTime Criacao { get; set; }
        public DateTime Atualizacao { get; set; }


        public SaidaMaterialArquivoModel DefinirAutor(Guid autor)
        {
            this.GuidAutor = autor;
            return this;
        }

        public SaidaMaterialArquivoModel DefinirId(int id){
            this.Id= id; 
            return this;
        }

        public SaidaMaterialArquivoModel DefinirBinarioId(long binarioId)
        {
            this.BinarioId = binarioId;
            return this;
        }

        public SaidaMaterialArquivoModel DefinirBinario(byte[] binario)
        {
            this.ArquivoBinario = binario;
            return this;
        }


        public SaidaMaterialArquivoModel DefinirMotivo(string motivo)
        {
            this.Motivo = motivo;
            return this;
        }

        public bool PodeEditar()
        {
            return !String.IsNullOrEmpty(this.Motivo);
        }
    }
}
