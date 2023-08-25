using ICE.GDocs.Domain.Core.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento
{
    public interface IAssinaturaCategoriaRepository : IRepository
    {
        Task<TryException<IEnumerable<AssinaturaCategoriaModel>>> Listar(IEnumerable<int> ListaIdsExclude = default, CancellationToken cancellationToken = default);
    }
}
