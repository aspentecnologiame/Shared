namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class FuncionalidadeModel
    {
        public int Id { get; set; }
        public string Chave { get; set; }
        public string Texto { get; set; }
        public string Icone { get; set; }
        public string Rota { get; set; }
        public int TipoFuncionalidadeId { get; set; }
        public int? IdPai { get; set; }
        public int Ordem { get; set; }

    }
}
