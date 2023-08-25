namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class SolicitacaoCienciaItemModel
    {
        public int IdItem { get; set; }
        public int IdSolicitacaoSaidaMaterialItem { get; set; }
        public int Quantidade { get; set; }
        public string Unidade { get; set; }
        public string Patrimonio { get; set; }
        public string Descricao { get; set; }
    }
}