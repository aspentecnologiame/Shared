using ICE.GDocs.Application.Core.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Application.GDocs
{
    public interface IDocToolsAppService: IApplicationService
    {
        Task<TryException<ArquivoModel>> Converter(string extensao, ArquivoModel arquivo, CancellationToken cancellationToken);
    }
}
