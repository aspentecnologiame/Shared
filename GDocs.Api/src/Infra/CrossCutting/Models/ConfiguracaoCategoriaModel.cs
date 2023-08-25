namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class ConfiguracaoCategoriaModel
    {
        public int Codigo { get; set; }
        public NumeracaoAutomaticaModel NumeracaoAutomatica { get; set; }
        public ConfiguracaoCategoriaPadraoModel CertificadoDigital { get; set; }
        public ConfiguracaoCategoriaPadraoModel AssinaturaDocumento { get; set; }
        public string ExtensoesPermitidas { get; set; }
    }

    public class ConfiguracaoCategoriaPadraoModel
    {
        public bool Habilitado { get; set; }
        public string Mustache { get; set; }
    }

    public class NumeracaoAutomaticaModel : ConfiguracaoCategoriaPadraoModel
    {
        public string ChaveSequence { get; set; }
    }
}
