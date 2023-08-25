using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.Services
{
    internal class AcessoService : DomainService, IAcessoService
    {
        private readonly IPerfilRepository _perfilRepository;

        public AcessoService(
            IUnitOfWork unitOfWork,
            IPerfilRepository perfilRepository
        ) : base(unitOfWork)
        {
            _perfilRepository = perfilRepository;
        }

        public async Task<TryException<IEnumerable<PerfilModel>>> ListarTodosOsPerfisAtivos(CancellationToken cancellationToken)
            => await _perfilRepository.ListarTodosPerfisAtivos(cancellationToken);
    }
}
