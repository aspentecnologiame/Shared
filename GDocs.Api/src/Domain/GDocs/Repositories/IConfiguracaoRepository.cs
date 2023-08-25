using ICE.GDocs.Domain.Core.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.Repositories
{
    public interface IConfiguracaoRepository : IRepository
    {
        Task<TryException<ConfiguracaoModel>> ObterConfiguracao(string chave, CancellationToken cancellationToken);
    }
}
