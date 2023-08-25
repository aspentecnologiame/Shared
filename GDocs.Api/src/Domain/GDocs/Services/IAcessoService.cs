using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.Services
{
    public interface IAcessoService : IDomainService
    {
        Task<TryException<IEnumerable<PerfilModel>>> ListarTodosOsPerfisAtivos(CancellationToken cancellationToken);
    }
}
