using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.Services
{
    public interface ISequencialService : IDomainService
    {
        Task<TryException<int>> ObterProximoSequencialStatusPagamento();

        Task<TryException<int>> ObterProximoSequencialSaidaMaterias();

        Task<TryException<int>> ObterProximoSequencialPorChaveCategoria(string chaveCategoria);
    }
}
