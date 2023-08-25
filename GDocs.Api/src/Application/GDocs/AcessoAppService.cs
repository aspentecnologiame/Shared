using ICE.GDocs.Domain.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ICE.GDocs.Application
{
    internal class AcessoAppService : IAcessoAppService
    {
        private readonly IAcessoService _acessoService;

        public AcessoAppService(
            IAcessoService acessoService
        )
        {
            _acessoService = acessoService;
        }

        public async System.Threading.Tasks.Task<TryException<IEnumerable<PerfilModel>>> ListarTodosOsPerfisAtivos(CancellationToken cancellationToken)
            => await _acessoService.ListarTodosOsPerfisAtivos(cancellationToken);
    }
}
