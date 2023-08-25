using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Domain.ExternalServices.Model;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.Services
{
    public interface IUsuarioService : IDomainService
    {
        Task<TryException<IEnumerable<UsuarioModel>>> ListarUsuariosAtivos(string nome, int IdPerfil, CancellationToken cancellationToken);
        Task<TryException<UsuarioModel>> InserirUsuario(UsuarioModel usuarioModel, CancellationToken cancellationToken);
        Task<TryException<UsuarioModel>> AlterarUsuario(UsuarioModel usuarioModel, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<UsuarioActiveDirectory>>> ListarUsuariosActiveDirectory(string nome, CancellationToken cancellationToken);
        Task<TryException<UsuarioActiveDirectory>> ObterUsuarioActiveDirectoryPorId(Guid usuarioAd, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<UsuarioModel>>> ListarUsuariosPorPerfil(Perfil perfil, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<UsuarioModel>>> ListarPerfisUsuariosPorGuids(IEnumerable<Guid> activeDirectoryIds, CancellationToken cancellationToken);
        Task<TryException<UsuarioModel>> AutenticarUsuario(string nomeUsuario, string senha, CancellationToken cancellationToken);
        Task<TryException<UsuarioModel>> ObterPermissoesUsuarioPorActiveDirectoryId(Guid activeDirectoryId, CancellationToken cancellationToken);
        Task<TryException<UsuarioModel>> AutenticarUsuarioPorNomeUsuario(string nomeUsuario, CancellationToken cancellationToken);
        Task<TryException<UsuarioModel>> ListarPerfisUsuarioPorGuid(Guid activeDirectoryId, CancellationToken cancellationToken);
    }
}
