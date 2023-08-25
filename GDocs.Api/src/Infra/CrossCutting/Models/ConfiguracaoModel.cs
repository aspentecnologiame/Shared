namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class ConfiguracaoModel
    {
        public int Id { get; set; }
        public string Chave { get; set; }
        public string Valor { get; set; }
        public string Descricao { get; set; }
        public bool Ativo { get; set; }
    }
}
