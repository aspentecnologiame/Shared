using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.ExternalServices
{
    public interface IDocToolsExternalService
    {
        Task<TryException<ArquivoModel>> Converter(string extensao, ArquivoModel arquivo, CancellationToken cancellationToken);
    }
}
