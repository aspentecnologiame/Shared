using ICE.GDocs.Domain.Core.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento
{
    public interface IProcessoAssinaturaDocumentoOrigemRepository : IRepository
    {
        Task<TryException<ProcessoAssinaturaDocumentoOrigemModel>> ListarPorProcessoAssinaturaDocumentoOrigemIdENome(string ProcessoAssinaturaDocumentoOrigemId, string ProcessoAssinaturaDocumentoOrigemNome, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<ProcessoAssinaturaDocumentoOrigemModel>>> ListarProcessosAssinaturaPorOrigem(string identificadorOrigem, string nomeOrigem, CancellationToken cancellationToken);
        Task<TryException<Return>> Inserir(ProcessoAssinaturaDocumentoOrigemModel processoAssinaturaDocumentoOrigemModel, CancellationToken cancellationToken);

        Task<TryException<Return>> InativarAssDocumentoOrigem(int processoAssinaturaDocumentoId, CancellationToken cancellationToken);

    }
}
