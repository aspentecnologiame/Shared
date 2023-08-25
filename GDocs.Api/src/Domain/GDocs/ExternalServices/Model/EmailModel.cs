using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Domain.ExternalServices.Model
{
    public class EmailModel
    {
        public string Nome { get; set; } = string.Empty;
        public string Responsavel { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string DataSaida { get; set; } = string.Empty;
        public string DataEnvio { get; set; } = string.Empty;
        public byte[] ArquivoBinario { get; set; } 
        public string Template { get; set; } = string.Empty;
        public string Destinatario { get; set; } = string.Empty;
        public string Assunto { get; set; } = string.Empty;
        public string CaminhoArquivoAnexo { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public List<RejeicaoDiretorModel> Texto => _Texto;
        private readonly List<RejeicaoDiretorModel> _Texto;

        public EmailModel()
        {
            _Texto = new List<RejeicaoDiretorModel>();
        }

        
    }
}
