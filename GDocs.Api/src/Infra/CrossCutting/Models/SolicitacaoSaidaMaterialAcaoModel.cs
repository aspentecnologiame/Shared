using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class SolicitacaoSaidaMaterialAcaoModel
    {
        public int Id { get; set; }
        public int IdSolicitacaoSaidaMaterial { get; set; }
        public int IdSaidaMaterialTipoAcao { get; set; }
        public int SolicitacaoMaterialAcaoItemId { get; set; }
        public string Conferente { get; set; }
        public DateTime? DataAcao { get; set; } = DateTime.Now;
        public string Portador { get; set; }
        public string SetorEmpresa { get; set; }
        public string Observacao { get; set; }
        public bool FlgAtivo { get; set; } = true;
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public List<SolicitacaoSaidaMaterialAcaoItemModel> AcaoItems => _acaoItems;

        private readonly List<SolicitacaoSaidaMaterialAcaoItemModel> _acaoItems;
        public SolicitacaoSaidaMaterialAcaoModel()
        {
            _acaoItems = new List<SolicitacaoSaidaMaterialAcaoItemModel>();
        }
        public SolicitacaoSaidaMaterialAcaoModel DefinirId(int idAcao)
        {
            Id = idAcao;
            return this;
        }
    }
}
