using ICE.GDocs.Application.GDocs.SaidaMaterialNotaFiscal.Interface;
using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Domain.GDocs.Services.SaidaMaterialNotaFiscal.Interface;
using ICE.GDocs.Domain.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Application.GDocs.SaidaMaterialNotaFiscal.Service
{
    internal class SolicitacaoCienciaNotaFiscalAppService : ISolicitacaoCienciaNotaFiscalAppService
    {
        private readonly ISolicitacaoCienciaNotaFiscalService _solicitacaoCienciaNotaFiscalService;
        private readonly IActiveDirectoryExternalService _activeDirectoryExternalService;
        private readonly IUsuarioService _usuarioService;


        public SolicitacaoCienciaNotaFiscalAppService(ISolicitacaoCienciaNotaFiscalService solicitacaoCienciaNotaFiscalService, IUsuarioService usuarioService, IActiveDirectoryExternalService activeDirectoryExternalService)
        {
            _solicitacaoCienciaNotaFiscalService = solicitacaoCienciaNotaFiscalService;
            _usuarioService = usuarioService;
            _activeDirectoryExternalService = activeDirectoryExternalService;
        }
        public async Task<TryException<IEnumerable<ProcessoAssinaturaDocumentoModel>>> ListarCienciasPendentesParaAprovacaoPeloUsuario(Guid activeDirectoryId, CancellationToken cancellationToken) =>
            await _solicitacaoCienciaNotaFiscalService.ListarCienciasPendentesParaAprovacaoPeloUsuario(activeDirectoryId, cancellationToken);

        public async Task<TryException<IEnumerable<CienciaUsuarioAprovacaoModel>>> ListarUsuarioAprovacao(int IdSolicitacaoCienciaNf, CancellationToken cancellationToken)
        {
            var result = await  _solicitacaoCienciaNotaFiscalService.ListarUsuarioAprovacao(IdSolicitacaoCienciaNf, cancellationToken);

            if (result.IsFailure)
                return result.Failure;

            var usuario =   _activeDirectoryExternalService.GetActiveDirectoryUsers(string.Empty,result.Success.Select(x => x.UsuarioGuid).AsList());

            if (usuario.IsFailure)
                return usuario.Failure;

            var resultComNome = result.Success.Join(usuario.Success,
                r => r.UsuarioGuid,
                u => u.Guid,
                (result, usuAd) => result.DefinirNome(usuAd.Nome));
                

            return resultComNome.ToCollection();
        }

        public async Task<TryException<SaidaMaterialNotaFiscalCienciaModel>> ObterCienciaNfPorId(int IdSolicitacaoCienciaNf, CancellationToken cancellationToken) { 
        
            var result = await _solicitacaoCienciaNotaFiscalService.ObterCienciaNfPorId(IdSolicitacaoCienciaNf, cancellationToken);

            if (result.IsFailure)
                return result.Failure;

            var usuario = await _usuarioService.ObterUsuarioActiveDirectoryPorId(result.Success.IdUsuario, cancellationToken);

            if (usuario.IsFailure)
                return usuario.Failure;

            result.Success.DefinirNomeUsuario(usuario.Success.Nome);

            return result;

        }

        public async Task<TryException<Return>> RegistroCienciaPorUsuario(SaidaMaterialNotaFiscalCienciaModel solicitacaoCienciaNfModel, Guid usuarioLogadoId, CancellationToken cancellationToken) => await
            _solicitacaoCienciaNotaFiscalService.RegistroCienciaPorUsuario(solicitacaoCienciaNfModel, usuarioLogadoId, cancellationToken);
    }
}
