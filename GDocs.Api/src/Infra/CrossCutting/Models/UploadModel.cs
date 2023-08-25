namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class UploadModel
    {
        public string NomeOriginal { get; set; }

        public string NomeSalvo { get; set; }

        public string ArquivoBinario { get; set; }

        public int? NumeracaoAutomatica { get; set; }
    }
}
