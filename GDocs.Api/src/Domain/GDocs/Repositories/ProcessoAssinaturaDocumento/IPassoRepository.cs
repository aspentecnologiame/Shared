using ICE.GDocs.Domain.Core.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento
{
    public interface IPassoRepository : IRepository
    {
        Task<TryException<IEnumerable<AssinaturaPassoItemModel>>> Salvar(int processoAssinaturaDocumentoId, IEnumerable<AssinaturaPassoItemModel> assinaturaPassoItems, CancellationToken cancellationToken);
        Task<TryException<Return>> InativarPassos(int processoAssinaturaDocumentoId, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<AssinaturaPassoItemModel>>> ObterPassosEhUsuariosPorId(int processoAssinaturaDocumentoId, CancellationToken cancellationToken);

    }
}
