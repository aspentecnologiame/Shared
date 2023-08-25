namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class ArquivoModel
    {
        public byte[] Binario { get; set; }
        public string Extensao { get; set; }
        public int? NumeracaoAutomatica { get; set; }
    }
}
