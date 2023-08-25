using System.Collections.Generic;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class AssinaturaPassoModel
    {
        public bool AdicionarDir { get; set; }
        public bool NotificarFinalizacaoDir { get; set; }
        public IList<AssinaturaPassoItemModel> Itens { get; private set; } = new List<AssinaturaPassoItemModel>();
        public IList<AssinaturaPassoItemModel> ItensDir { get; private set; } = new List<AssinaturaPassoItemModel>();

        public void DefinirItens(IList<AssinaturaPassoItemModel> itens)
          => this.Itens = itens;

        public void DefinirItensDir(IList<AssinaturaPassoItemModel> itensDir)
          => this.ItensDir = itensDir;
    }
}
