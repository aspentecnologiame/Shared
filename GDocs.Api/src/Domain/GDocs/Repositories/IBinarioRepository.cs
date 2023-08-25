using ICE.GDocs.Domain.Core.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.Repositories
{
    public interface IBinarioRepository : IRepository
    {
        Task<TryException<long>> Inserir(byte[] binario, CancellationToken cancellationToken);

        Task<TryException<byte[]>> ObterPorId(long binarioId, CancellationToken cancellationToken);
    }
}
