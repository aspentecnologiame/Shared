using ICE.GDocs.Domain.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Application
{
    internal class ConfiguracaoAppService : IConfiguracaoAppService
    {
        private readonly IConfiguracaoRepository _configuracaoRepository;

        public ConfiguracaoAppService(
            IConfiguracaoRepository configuracaoRepository
        )
        {
            _configuracaoRepository = configuracaoRepository;
        }

        public async Task<TryException<ConfiguracaoModel>> ObterConfiguracaoRepositorio(string chaveConfiguracao, CancellationToken cancellationToken)
            => await _configuracaoRepository.ObterConfiguracao(chaveConfiguracao, cancellationToken);
    }
}
