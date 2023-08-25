using ICE.GDocs.Domain.Core.Repositories;
using System;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.Repositories
{
    public interface ISequencialRepository : IRepository
    {
        Task<TryException<int>> ObterProximo(string chave);
    }
}
