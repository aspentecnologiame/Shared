namespace ICE.GDocs.Common.Core.Domain.ValueObjects
{
    public class MensagemLog
    {
        public MensagemLog(string nomeUsuario, string texto, string origem = "API")
        {            
            Texto = texto;
            NomeUsuario = nomeUsuario;
            Origem = origem;
        }

        public string Origem { get; private set; }
        public string NomeUsuario { get; private set; }
        public string Texto { get; private set; }
    }
}
