using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Threading.Tasks;

namespace ICE.GDocs.Infra.ExternalServices.ArmazenamentoTemporario
{
    public class GDocsCacheExternalService : IGDocsCacheExternalService
    {
        private readonly IRedisDatabase _redisDatabase;



        public GDocsCacheExternalService(IRedisClientFactory cacheClient, IConfiguration configuration)
        {
            _redisDatabase = cacheClient.GetDefaultRedisClient().GetDb(configuration.GetValue("Redis:Database", 0));

        }

        public async Task AdicionarRefreshToken(RefreshTokenModel refreshTokenModel, TimeSpan tempoDeExpiracao)
            => await InserirValor(
                $"Login/{refreshTokenModel.RefreshToken}/RefreshToken",
                refreshTokenModel,
                tempoDeExpiracao);

        public async Task RemoverRefreshToken(string refreshToken)
            => await RemoverRegistro($"Login/{refreshToken}/RefreshToken");

        public async Task<TryException<RefreshTokenModel>> ObterRefreshToken(string guidToken)
            => await ObterValor<RefreshTokenModel>($"Login/{guidToken}/RefreshToken");

        public async Task<TryException<Guid>> ObterGuidUsuarioAd(string chaveAcesso)
        {
            var guidArmazenada = await ObterValor($"ChaveAcessoPorEmail/{chaveAcesso}/GuidUsuarioAd");

            if (guidArmazenada.IsFailure)
                return guidArmazenada.Failure;

            if (string.IsNullOrEmpty(guidArmazenada.Success))
                return Guid.Empty;

            return new Guid(guidArmazenada.Success);
        }

        public async Task RemoverGuidUsuarioAd(string chaveAcesso)
            => await RemoverRegistro($"ChaveAcessoPorEmail/{chaveAcesso}/GuidUsuarioAd");

        public async Task<TryException<Guid>> AdicionarFiltrosDocumentoFI1548ParaReportServer(DocumentoFI1548FilterModel documentoFI1548FilterModel, TimeSpan tempoDeExpiracao)
        {
            var chave = Guid.NewGuid();

            await InserirValor(
                $"ReporteServer/DocumentoFI1548/{chave}/filtro",
                documentoFI1548FilterModel,
                tempoDeExpiracao);

            return chave;
        }

        public async Task<TryException<Guid>> AdicionarFiltrosGerenciamentoAssinaturaParaReportServer(AssinaturaInformacoesFilterModel assinaturaInformacoesFilterModel, TimeSpan tempoDeExpiracao)
        {
            var chave = Guid.NewGuid();

            await InserirValor(
                $"ReporteServer/GerenciamentoAssinatura/{chave}/filtro",
                assinaturaInformacoesFilterModel,
                tempoDeExpiracao);

            return chave;
        }

        public async Task<TryException<Guid>> AdicionarFiltrosSolicitacoesSaidaMaterialParaReportServer(SolicitacaoSaidaMaterialFilterModel solicitacaoSaidaMaterialFilterModel, TimeSpan tempoDeExpiracao)
        {
            var chave = Guid.NewGuid();

            await InserirValor(
                $"ReporteServer/SolicitacoesSaidaMaterial/{chave}/filtro",
                solicitacaoSaidaMaterialFilterModel,
                tempoDeExpiracao);

            return chave;
        }

        public async Task<TryException<Guid>> AdicionarFiltrosSaidaMaterialNFParaReportServer(SaidaMaterialNotaFiscalFilterModel saidaMaterialNotaFiscalFilterModel, TimeSpan tempoDeExpiracao)
        {
            var chave = Guid.NewGuid();

            await InserirValor(
                $"ReporteServer/SaidaMaterialNotaFiscal/{chave}/filtro",
                saidaMaterialNotaFiscalFilterModel,
                tempoDeExpiracao);

            return chave;
        }

        public async Task<TryException<DocumentoFI1548FilterModel>> ObterFiltrosDocumentoFI1548ParaReportServer(Guid chave)
            => await ObterValor<DocumentoFI1548FilterModel>($"ReporteServer/DocumentoFI1548/{chave}/filtro");

        public async Task<TryException<AssinaturaInformacoesFilterModel>> ObterFiltrosGerenciamentoAssinaturaParaReportServer(Guid chave)
            => await ObterValor<AssinaturaInformacoesFilterModel>($"ReporteServer/GerenciamentoAssinatura/{chave}/filtro");

        public async Task<TryException<SolicitacaoSaidaMaterialFilterModel>> ObterFiltrosSolicitacoesSaidaMaterialParaReportServer(Guid chave)
            => await ObterValor<SolicitacaoSaidaMaterialFilterModel>($"ReporteServer/SolicitacoesSaidaMaterial/{chave}/filtro");

        public async Task<TryException<SaidaMaterialNotaFiscalFilterModel>> ObterFiltrosSaidaMaterialNotaFiscalParaReportServer(Guid chave)
            => await ObterValor<SaidaMaterialNotaFiscalFilterModel>($"ReporteServer/SaidaMaterialNotaFiscal/{chave}/filtro");

        public async Task<TryException<int>> AdicionarPassoUsuarioDownload(PassoUsuarioDownload passoUsuarioDownload, TimeSpan tempoDeExpiracao)
        {
            var chave = passoUsuarioDownload.PadId;
            var verificaPasso = await ObterPassoUsuarioDownload(passoUsuarioDownload.PadId);

            if (verificaPasso.Success?.UsuarioId != passoUsuarioDownload.UsuarioId)
                await InserirValor($"Gdocs/Passo/{chave}/filtro", passoUsuarioDownload, tempoDeExpiracao);

            return chave;
        }

        public async Task<TryException<PassoUsuarioDownload>> ObterPassoUsuarioDownload(int chave)
         => await ObterValor<PassoUsuarioDownload>($"Gdocs/Passo/{chave}/filtro");


        public async Task RemoverPassoUsuarioDownload(int chave, Guid usuarioId)
        {
            var verificaPasso = await ObterPassoUsuarioDownload(chave);
            if (verificaPasso.Success?.UsuarioId == usuarioId)
                await RemoverRegistro($"Gdocs/Passo/{chave}/filtro");
        }


        private async Task InserirValor(string chave, object valor, TimeSpan tempoDeExpiracao)
           => await _redisDatabase.AddAsync($"/{chave}", Newtonsoft.Json.JsonConvert.SerializeObject(valor), tempoDeExpiracao);

        private async Task<TryException<string>> ObterValor(string chave)
            => await _redisDatabase.GetAsync<string>($"/{chave}");

        private async Task<TryException<T>> ObterValor<T>(string chave) where T : class
        {
            var tokenArmazenado = await ObterValor(chave);

            if (tokenArmazenado.IsFailure || tokenArmazenado.Success == default)
                return tokenArmazenado.Failure;

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(tokenArmazenado.Success);
        }

        private async Task RemoverRegistro(string chave)
        {
            await _redisDatabase.RemoveAsync($"/{chave}");
        }
    }
}
