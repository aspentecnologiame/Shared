using ICE.GDocs.Domain.Core.Repositories;
using ICE.GDocs.Domain.ExternalServices.Model;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.Repositories
{
    public interface IPerfilRepository : IRepository
    {
        Task<TryException<IEnumerable<UsuarioModel>>> ListarPerfisUsuariosPorGuids(IEnumerable<Guid> activeDirectoryIds, CancellationToken cancellationToken);
        Task<TryException<UsuarioModel>> ListarPerfisUsuarioPorGuid(Guid activeDirectoryId, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<UsuarioModel>>> ListarPerfisUsuariosPorPerfil(int perfilId, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<PerfilModel>>> ListarTodosPerfisAtivos(CancellationToken cancellationToken);
        Task<TryException<UsuarioModel>> SalvarPerfisUsuario(UsuarioModel usuarioModel, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<FuncionalidadeModel>>> ListarFuncionalidadesPorPerfis(IEnumerable<int> perfis, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<UsuarioModel>>> ListarUsuariosPorPerfil(Perfil perfil, CancellationToken cancellationToken);
    }
}
