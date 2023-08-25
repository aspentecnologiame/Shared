using ICE.GDocs.Domain.Core.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento
{
    public interface ITemplateProcessoAssinaturaDocumentoRepository : IRepository
    {
        Task<TryException<IEnumerable<AssinaturaPassoItemModel>>> ListarPassosEUsuariosPorTag(string tag, CancellationToken cancellationToken);
    }
}
