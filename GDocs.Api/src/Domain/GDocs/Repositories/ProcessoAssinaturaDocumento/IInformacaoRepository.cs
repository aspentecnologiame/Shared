using ICE.GDocs.Domain.Core.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento
{
    public interface IInformacaoRepository : IRepository
    {
        Task<TryException<IEnumerable<AssinaturaInformacoesModel>>> Listar(AssinaturaInformacoesFilterModel filtro, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<AssinaturaNomeDocumento>>> ListarNomesDocumentos(string termo, CancellationToken cancellationToken);
        Task<TryException<AssinaturaModel>> ObterProcessoComPassosParaAssinatura(long processoId, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<AssinaturaInformacoesModel>>> AtualizarStatusProcessos(IEnumerable<int> processoListId, StatusAssinaturaDocumento status, CancellationToken cancellationToken);
        Task<TryException<AssinaturaInformacoesModel>> Salvar(AssinaturaInformacoesModel assinaturaInformacoesModel, CancellationToken cancellationToken);

        Task<TryException<Return>> InativarInformacao(int processoAssinaturaDocumentoId, CancellationToken cancellationToken);

    }
}
