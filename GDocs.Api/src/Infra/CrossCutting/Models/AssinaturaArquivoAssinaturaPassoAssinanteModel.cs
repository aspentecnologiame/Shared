using System.Collections.Generic;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class AssinaturaArquivoAssinaturaPassoAssinanteModel
    {
        public AssinaturaArquivoAssinaturaPassoAssinanteModel(
            IEnumerable<AssinaturaArquivoModel> assinaturaArquivoModel,
            IEnumerable<AssinaturaUsuarioModel> assinaturaUsuarioAd)
        {
            AssinaturaArquivos = assinaturaArquivoModel;
            AssinaturaUsuarioAd = assinaturaUsuarioAd;
        }

        public AssinaturaArquivoAssinaturaPassoAssinanteModel() { }

        public IEnumerable<AssinaturaArquivoModel> AssinaturaArquivos { get; set; }
        public IEnumerable<AssinaturaUsuarioModel> AssinaturaUsuarioAd { get; set; }

    }
}
