using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Domain.ExternalServices.Model;
using ICE.GDocs.Domain.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Application
{
    internal class UsuarioAppService : IUsuarioAppService
    {
        private const int ID_PERFIL_CONVIDADO = 1;

        private readonly IUsuarioService _usuarioService;
        private readonly IGDocsCacheExternalService _gDocsCacheExternalService;

        public UsuarioAppService(IUsuarioService usuarioService, IGDocsCacheExternalService gDocsCacheExternalService)
        {
            _usuarioService = usuarioService;
            _gDocsCacheExternalService = gDocsCacheExternalService;
        }

        public async Task<TryException<IEnumerable<UsuarioModel>>> ListarUsuarios(int idPerfil, string nome, CancellationToken cancellationToken)
            => await _usuarioService.ListarUsuariosAtivos(nome, idPerfil, cancellationToken);

        public async Task<TryException<UsuarioModel>> InserirUsuario(UsuarioModel usuarioModel, CancellationToken cancellationToken)
            => await _usuarioService.InserirUsuario(usuarioModel, cancellationToken);

        public async Task<TryException<UsuarioModel>> AlterarUsuario(UsuarioModel usuarioModel, CancellationToken cancellationToken)
            => await _usuarioService.AlterarUsuario(usuarioModel, cancellationToken);

        public async Task<TryException<IEnumerable<UsuarioActiveDirectory>>> ListarUsuariosActiveDirectory(string nome, CancellationToken cancellationToken)
        {
            var usersAD = await _usuarioService.ListarUsuariosActiveDirectory(nome, cancellationToken);

            if (usersAD.IsFailure)
                return usersAD.Failure;

            var usuariosAd = usersAD.Success;

            var usuariosPerfisDb = await _usuarioService.ListarPerfisUsuariosPorGuids(usuariosAd.Select(e => e.Guid), cancellationToken);

            if (usuariosPerfisDb.IsFailure)
                return usuariosPerfisDb.Failure;

            var usuariosPerfis = usuariosPerfisDb.Success;

            return (
                       from usuAd in usuariosAd
                       join usuPer in usuariosPerfis on usuAd.Guid equals usuPer.ActiveDirectoryId into usuPerLeft
                       from usuPer in usuPerLeft.DefaultIfEmpty()
                       where
                            usuPer == null || usuPer.Perfis.All(per => per.Id == ID_PERFIL_CONVIDADO)
                       select usuAd
                    ).ToList();
        }

        public async Task<TryException<IEnumerable<UsuarioActiveDirectory>>> ListarUsuariosActiveDirectory(Guid usuarioGuidAd, CancellationToken cancellationToken)
        {
            var obterUsuarioActiveDirectoryId = await _usuarioService.ObterUsuarioActiveDirectoryPorId(usuarioGuidAd, cancellationToken);

            if (obterUsuarioActiveDirectoryId.IsFailure)
                return obterUsuarioActiveDirectoryId.Failure;

            var usuariosAd = new List<UsuarioActiveDirectory> { obterUsuarioActiveDirectoryId.Success };

            var usuariosPerfisDb = await _usuarioService.ListarPerfisUsuariosPorGuids(usuariosAd.Select(e => e.Guid), cancellationToken);

            if (usuariosPerfisDb.IsFailure)
                return usuariosPerfisDb.Failure;

            var usuariosPerfis = usuariosPerfisDb.Success;

            return usuariosAd.Where(usuAd => usuariosPerfis.Any(usuPer => usuPer.ActiveDirectoryId == usuAd.Guid)).AsList();
        }

        public async Task<TryException<UsuarioModel>> AutenticarUsuario(string nomeUsuario, string senha, CancellationToken cancellationToken)
            => await _usuarioService.AutenticarUsuario(nomeUsuario, senha, cancellationToken);

        public async Task<TryException<UsuarioModel>> ObterUsuarioPorActiveDirectoryId(Guid activeDirectoryId, CancellationToken cancellationToken)
            => await _usuarioService.ObterPermissoesUsuarioPorActiveDirectoryId(activeDirectoryId, cancellationToken);

        public async Task<TryException<UsuarioModel>> AutenticarUsuarioPorNomeUsuario(string nomeUsuario, CancellationToken cancellationToken)
            => await _usuarioService.AutenticarUsuarioPorNomeUsuario(nomeUsuario, cancellationToken);

        public async Task<TryException<Return>> InserirRefreshToken(RefreshTokenModel refreshTokenModel, TimeSpan tempoDeExpiracao)
        {
            await _gDocsCacheExternalService.AdicionarRefreshToken(refreshTokenModel, tempoDeExpiracao);

            return Return.Empty;
        }

        public async Task<TryException<Return>> RemoverRefreshToken(string refreshToken)
        {
            await _gDocsCacheExternalService.RemoverRefreshToken(refreshToken);

            return Return.Empty;
        }

        public async Task<TryException<RefreshTokenModel>> ObterRefreshToken(string guidToken)
            => await _gDocsCacheExternalService.ObterRefreshToken(guidToken);


        public async Task<TryException<Guid>> ObterGuidUsuarioAdPorChaveDeAcessoEmail(string chaveAcesso)
            => await _gDocsCacheExternalService.ObterGuidUsuarioAd(chaveAcesso);


        public async Task<TryException<Return>> RemoverChaveDeAcessoEmail(string chaveAcesso)
        {
            await _gDocsCacheExternalService.RemoverGuidUsuarioAd(chaveAcesso);

            return Return.Empty;
        }
    }
}
