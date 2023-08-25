using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal
{
    public class SaidaMaterialNotaFiscalAcaoModel
    {
        public int Id { get; set; }
        public int IdSaidaMaterialNotaFiscal { get; set; }
        public int IdSaidaMaterialNotaFiscalTipoAcao { get; set; }
        public int IdSaidaMaterialNotaFiscalAcaoItem { get; set; }
        public string Conferente { get; set; }
        public DateTime? DataAcao { get; set; } = DateTime.Now;
        public string Portador { get; set; }
        public string SetorEmpresa { get; set; }
        public string Observacao { get; set; }
        public bool FlgAtivo { get; set; } = true;
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public List<SaidaMaterialNotaFiscalAcaoItemModel> AcaoItems => _acaoItems;

        private readonly List<SaidaMaterialNotaFiscalAcaoItemModel> _acaoItems;
        public SaidaMaterialNotaFiscalAcaoModel()
        {
            _acaoItems = new List<SaidaMaterialNotaFiscalAcaoItemModel>();
        }

        public SaidaMaterialNotaFiscalAcaoModel DefinirId(int idAcao)
        {
            Id = idAcao;
            return this;
        }
    }
}
