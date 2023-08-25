using System.Collections.Generic;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class AssinaturaModel
    {
        public AssinaturaModel()
        {
            Passos = new AssinaturaPassoModel();
        }

        public AssinaturaInformacoesModel Informacoes { get; set; }
        public AssinaturaPassoModel Passos { get; set; }        
        public IEnumerable<AssinaturaArquivoModel> Arquivos { get; set; }
    }
}
