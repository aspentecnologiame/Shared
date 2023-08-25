using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Application.GDocs
{
    internal class DocToolsAppService : IDocToolsAppService
    {
        private readonly IDocToolsExternalService _docToolsExternalService;

        public DocToolsAppService(IDocToolsExternalService docToolsExternalService)
        {
            _docToolsExternalService = docToolsExternalService;
        }

        public async Task<TryException<ArquivoModel>> Converter(string extensao, ArquivoModel arquivo, CancellationToken cancellationToken)
        => await _docToolsExternalService.Converter(extensao, arquivo, cancellationToken);
    }
}
