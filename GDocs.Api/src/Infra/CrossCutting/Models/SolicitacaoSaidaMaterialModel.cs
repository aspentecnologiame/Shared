using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using System;
using System.Collections.Generic;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class SolicitacaoSaidaMaterialModel
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public string TipoSaida { get; set; }
        public bool FlgRetorno { get; set; }
        public Guid GuidResponsavel { get; set; }
        public Guid Autor { get; set; }
        public string NomeResponsavel { get; set; }
        public string SetorResponsavel { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public DateTime? Retorno { get; set; }
        public DateTime? DataAcao { get; set; }
        public string DataRetornoFormatada { get; set; }
        public string Motivo { get; set; }
        public string Observacao { get; set; }
        public bool Ativo { get; set; }
        public long BinarioId { get; set; }
        public int StatusId { get; set; }
        public string Status { get; set; }
        public string HtmlItens { get; set; }
        public DateTime DataCriacao { get; set; }
        public string DataCriacaoFormatada { get; set; }
        public string DataSaidaFormatada { get; set; }
        public bool RetornoParcial { get; set; }
        public List<SolicitacaoSaidaMaterialItemModel> ItemMaterial => _itemMaterial;

        private readonly List<SolicitacaoSaidaMaterialItemModel> _itemMaterial;
        public SolicitacaoSaidaMaterialModel()
        {
            _itemMaterial = new List<SolicitacaoSaidaMaterialItemModel>();
        }

        public SolicitacaoSaidaMaterialModel DefinirNumeroMaterial(int numero)
         {
            Numero = numero;
            return this;
        }

        public SolicitacaoSaidaMaterialModel DefinirMaterialId(int id)
        {
            Id = id;
            return this;
        }

        public SolicitacaoSaidaMaterialModel DefinirNomeResponsavel(string nomeResponsavel)
        {
            NomeResponsavel = nomeResponsavel;
            return this;
        }

        public SolicitacaoSaidaMaterialModel DefinirAutorEResponsavel(Guid UsuarioLogado)
        {
            Autor = UsuarioLogado;
            GuidResponsavel= UsuarioLogado;
            return this;
        }

        public SolicitacaoSaidaMaterialModel DefinirComoCancelado()
        {
            Status = SolicitacaoSaidaMaterialStatus.Cancelado.GetDescription();
            StatusId = SolicitacaoSaidaMaterialStatus.Cancelado.ToInt32();
            return this;
        }

        public SolicitacaoSaidaMaterialModel DefinirBinario(long id)
        {
            BinarioId = id;
            return this;
        }

        public bool PodeEditar()
        {
            return Id != 0;
        }

        public bool ExisteBinario()
        {
            return BinarioId != 0;
        }
    }
}
