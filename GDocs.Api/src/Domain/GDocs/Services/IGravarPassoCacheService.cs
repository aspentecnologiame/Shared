using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ICE.GDocs.Domain.Core.Services;

namespace ICE.GDocs.Domain.GDocs.Services
{
    public interface IGravarPassoCacheService : IDomainService
    {
        Task<TryException<int>> GravarPassoUsuario(int padId, Guid usuarioId, CancellationToken cancellationToken);
        Task<TryException<PassoUsuarioDownload>> ObterPassoPorId(int chave);
        Task RemoverPassoPorId(int chave, Guid usuarioId);
    }
}
