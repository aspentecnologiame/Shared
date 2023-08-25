using ICE.GDocs.Application.Core.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Application
{
    public interface IConfiguracaoAppService : IApplicationService
    {
        Task<TryException<ConfiguracaoModel>> ObterConfiguracaoRepositorio(string chaveConfiguracao, CancellationToken cancellationToken);
    }
}
