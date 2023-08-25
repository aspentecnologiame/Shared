using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Domain.GDocs.Repositories.SolicitacaoCiencia;
using ICE.GDocs.Domain.GDocs.Services.SolicitacaoSaidaMaterial;
using ICE.GDocs.Domain.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Application.GDocs.SolicitacaoSaidaMaterial
{
    internal class SolicitacaoCienciaAppService : ISolicitacaoCienciaAppService
    {
        private readonly ISolicitacaoSaidaMaterialAppService _solicitacaoSaidaMaterialAppService;
        private readonly ISolicitacaoCienciaService _solicitacaoCienciaService;
        private readonly ISolicitacaoCienciaRepository _solicitacaoCienciaRepository;
        private readonly IUsuarioService _usuarioService;
        private readonly IActiveDirectoryExternalService _activeDirectoryExternalService;


        public SolicitacaoCienciaAppService(
            ISolicitacaoSaidaMaterialAppService solicitacaoSaidaMaterialAppService,
            ISolicitacaoCienciaService solicitacaoCienciaService,
            ISolicitacaoCienciaRepository solicitacaoCienciaRepository,
            IUsuarioService usuarioService,
            IActiveDirectoryExternalService activeDirectoryExternalService)
        {
            _solicitacaoSaidaMaterialAppService = solicitacaoSaidaMaterialAppService;
            _solicitacaoCienciaService = solicitacaoCienciaService;
            _solicitacaoCienciaRepository = solicitacaoCienciaRepository;
            _usuarioService = usuarioService;
            _activeDirectoryExternalService = activeDirectoryExternalService;
        }

        public async Task<TryException<IEnumerable<ProcessoAssinaturaDocumentoModel>>> ListarCienciasPendentesDeAprovacaoPeloUsuario(Guid activeDirectoryId, CancellationToken cancellationToken)
            => await _solicitacaoCienciaService.ListarCienciasPendentesDeAprovacaoPeloUsuario(activeDirectoryId, cancellationToken);

        public async Task<TryException<IEnumerable<CienciaUsuarioAprovacaoModel>>> ListarObsPorUsuario(int IdSolicitacaoCiencia, CancellationToken cancellationToken) 
        {
            var result = await _solicitacaoCienciaService.ListarObsPorUsuario(IdSolicitacaoCiencia, cancellationToken);

            if (result.IsFailure)
                return result.Failure;

            var usuarioAd = _activeDirectoryExternalService.GetActiveDirectoryUsers(string.Empty, result.Success.Select(x => x.UsuarioGuid).AsList());

            if (usuarioAd.IsFailure)
                return usuarioAd.Failure;

            var usuariosAprovacao = result.Success.Join(usuarioAd.Success,
                r => r.UsuarioGuid,
                u => u.Guid,
                (result, usuAd) => result.DefinirNome(usuAd.Nome));


            return usuariosAprovacao.ToCollection();

        }

        public async Task<TryException<SoclicitacaoCienciaAprovadoresModel>> ObterAprovadoresCienciaCancelamento(int solicitacaoSaidaMaterialId, CancellationToken cancellationToken) 
        {
            var aprovadores = await _solicitacaoCienciaService.ObterAprovadoresCienciaCancelamento(solicitacaoSaidaMaterialId, cancellationToken);

            if (aprovadores.IsFailure)
                return aprovadores.Failure;

            if (aprovadores.Success == null)
                return aprovadores;

            var usuarioAd = _activeDirectoryExternalService.GetActiveDirectoryUsers(string.Empty, aprovadores.Success.CienciaUsuariosAprovacao.Select(x => x.UsuarioGuid).AsList());

            if (usuarioAd.IsFailure)
                return usuarioAd.Failure;

             aprovadores.Success.CienciaUsuariosAprovacao.Join(usuarioAd.Success,
                r => r.UsuarioGuid,
                u => u.Guid,
                (result, usuAd) => result.DefinirNome(usuAd.Nome)).ToCollection();


            return aprovadores;

        }

        public async Task<TryException<SolicitacaoCienciaModel>> ObterCienciaPeloIdentificador(int idSolicitacaoCiencia, CancellationToken cancellationToken)
        {

            var result = await _solicitacaoCienciaRepository.ObterCienciaPeloIdentificador(idSolicitacaoCiencia, cancellationToken);

            if (result.IsFailure)
                return result.Failure;

            var usuarios = await _usuarioService.ObterUsuarioActiveDirectoryPorId(result.Success.IdUsuario, cancellationToken);

            if (usuarios.IsFailure)
                return usuarios.Failure;

            result.Success.DefinirNomeUsuario(usuarios.Success.Nome);

            return result;
        }

        public async Task<TryException<Return>> RegistroCienciaPorUsuario(SolicitacaoCienciaModel solicitacaoCienciaModel, UsuarioModel usuarioModel, CancellationToken cancellationToken)
        {
            var result = await _solicitacaoCienciaService.RegistroCienciaPorUsuario(solicitacaoCienciaModel, usuarioModel, cancellationToken);

            if (result.IsFailure)
                return result.Failure;

            if (result.Success && solicitacaoCienciaModel.IdTipoCiencia == (int)TipoCiencia.Cancelamento)
                await _solicitacaoSaidaMaterialAppService.CancelarOuSolicitarCiencia(solicitacaoCienciaModel.IdSolicitacaoSaidaMaterial, null, usuarioModel, true, cancellationToken);

            return Return.Empty;
        }
    }
}
