using ICE.GDocs.Domain.Core.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento
{
    public interface IArquivoRepository : IRepository
    {
        Task<TryException<IEnumerable<AssinaturaArquivoModel>>> Salvar(int processoAssinaturaDocumentoId, IEnumerable<AssinaturaArquivoModel> arquivos,CancellationToken cancellationToken, bool edicao = false);
        Task<TryException<int>> Inativar(long id, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<AssinaturaArquivoModel>>> ListarArquivosUploadPorPadId(int processoAssinaturaDocumentoId, CancellationToken cancellationToken, bool listarTodos = false);
        Task<TryException<IEnumerable<AssinaturaArquivoModel>>> ListarArquivoExpurgoPorPad(int processoAssinaturaDocumentoId, CancellationToken cancellationToken, bool listarTodos = false);
    }
}
