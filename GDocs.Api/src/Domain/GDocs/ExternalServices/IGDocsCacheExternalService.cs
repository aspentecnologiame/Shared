using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using System;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.ExternalServices
{
    public interface IGDocsCacheExternalService
    {
        Task AdicionarRefreshToken(RefreshTokenModel refreshTokenModel, TimeSpan tempoDeExpiracao);
        Task RemoverRefreshToken(string refreshToken);
        Task<TryException<RefreshTokenModel>> ObterRefreshToken(string guidToken);
        Task<TryException<Guid>> ObterGuidUsuarioAd(string chaveAcesso);
        Task RemoverGuidUsuarioAd(string chaveAcesso);
        Task<TryException<Guid>> AdicionarFiltrosDocumentoFI1548ParaReportServer(DocumentoFI1548FilterModel documentoFI1548FilterModel, TimeSpan tempoDeExpiracao);
        Task<TryException<Guid>> AdicionarFiltrosGerenciamentoAssinaturaParaReportServer(AssinaturaInformacoesFilterModel assinaturaInformacoesFilterModel, TimeSpan tempoDeExpiracao);
        Task<TryException<Guid>> AdicionarFiltrosSolicitacoesSaidaMaterialParaReportServer(SolicitacaoSaidaMaterialFilterModel solicitacaoSaidaMaterialFilterModel, TimeSpan tempoDeExpiracao);
        Task<TryException<Guid>> AdicionarFiltrosSaidaMaterialNFParaReportServer(SaidaMaterialNotaFiscalFilterModel saidaMaterialNotaFiscalFilterModel, TimeSpan tempoDeExpiracao);
        Task<TryException<DocumentoFI1548FilterModel>> ObterFiltrosDocumentoFI1548ParaReportServer(Guid chave);
        Task<TryException<AssinaturaInformacoesFilterModel>> ObterFiltrosGerenciamentoAssinaturaParaReportServer(Guid chave);
        Task<TryException<SolicitacaoSaidaMaterialFilterModel>> ObterFiltrosSolicitacoesSaidaMaterialParaReportServer(Guid chave);
        Task<TryException<SaidaMaterialNotaFiscalFilterModel>> ObterFiltrosSaidaMaterialNotaFiscalParaReportServer(Guid chave);
        Task<TryException<int>> AdicionarPassoUsuarioDownload(PassoUsuarioDownload passoUsuarioDownload, TimeSpan tempoDeExpiracao);
        Task<TryException<PassoUsuarioDownload>> ObterPassoUsuarioDownload(int chave);
        Task RemoverPassoUsuarioDownload(int chave, Guid usuarioId);
    }
}
