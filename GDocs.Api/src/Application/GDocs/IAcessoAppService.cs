using ICE.GDocs.Application.Core.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Application
{
    public interface IAcessoAppService : IApplicationService
    {
        Task<TryException<IEnumerable<PerfilModel>>> ListarTodosOsPerfisAtivos(CancellationToken cancellationToken);
    }
}
