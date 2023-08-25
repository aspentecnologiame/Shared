using ICE.GDocs.Application.Core.Services;
using ICE.GDocs.Domain.ExternalServices.Model;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Application
{
    public interface IUsuarioAppService : IApplicationService
    {
        Task<TryException<IEnumerable<UsuarioModel>>> ListarUsuarios(int idPerfil, string nome, CancellationToken cancellationToken);
        Task<TryException<UsuarioModel>> InserirUsuario(UsuarioModel usuarioModel, CancellationToken cancellationToken);
        Task<TryException<UsuarioModel>> AlterarUsuario(UsuarioModel usuarioModel, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<UsuarioActiveDirectory>>> ListarUsuariosActiveDirectory(string nome, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<UsuarioActiveDirectory>>> ListarUsuariosActiveDirectory(Guid usuarioGuidAd, CancellationToken cancellationToken);
        Task<TryException<UsuarioModel>> AutenticarUsuario(string nomeUsuario, string senha, CancellationToken cancellationToken);
        Task<TryException<UsuarioModel>> ObterUsuarioPorActiveDirectoryId(Guid activeDirectoryId, CancellationToken cancellationToken);
        Task<TryException<UsuarioModel>> AutenticarUsuarioPorNomeUsuario(string nomeUsuario, CancellationToken cancellationToken);
        Task<TryException<Return>> InserirRefreshToken(RefreshTokenModel refreshTokenModel, TimeSpan tempoDeExpiracao);
        Task<TryException<Return>> RemoverRefreshToken(string refreshToken);
        Task<TryException<RefreshTokenModel>> ObterRefreshToken(string guidToken);
        Task<TryException<Guid>> ObterGuidUsuarioAdPorChaveDeAcessoEmail(string chaveAcesso);
        Task<TryException<Return>> RemoverChaveDeAcessoEmail(string chaveAcesso);
    }
}
