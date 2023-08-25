using ICE.GDocs.Domain.Core.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.GDocs.Repositories.SolicitacaoSaidaMaterial
{
    public interface ISolicitacaoSaidaMaterialRepository : IRepository
    {
        Task<TryException<SolicitacaoSaidaMaterialModel>> ObterPorId(int id, CancellationToken cancellationToken);
        Task<TryException<SolicitacaoSaidaMaterialModel>> ObterMaterialComItensPorId(int id, CancellationToken cancellationToken);

        Task<TryException<SolicitacaoSaidaMaterialModel>> Inserir(SolicitacaoSaidaMaterialModel solicitacaoSaidaMaterialModel, CancellationToken cancellationToken);
        Task<TryException<SolicitacaoSaidaMaterialModel>> Atualizar(SolicitacaoSaidaMaterialModel solicitacaoSaidaMaterialModel, CancellationToken cancellationToken);
        Task<TryException<Return>> AtualizarStatus(int solicitacaoSaidaMaterialId, SolicitacaoSaidaMaterialStatus novoStatus, CancellationToken cancellationToken, bool retornoParcial = false);

        Task<TryException<Return>> AtualizarDataStatus(SolicitacaoCienciaModel solicitacaoCienciaModel, SolicitacaoSaidaMaterialStatus novoStatus, CancellationToken cancellationToken);
        Task<TryException<Return>> Excluir(int id);
        Task<TryException<IEnumerable<SolicitacaoSaidaMaterialModel>>> Listar(SolicitacaoSaidaMaterialFilterModel filtro, CancellationToken cancellationToken);

        Task<TryException<IEnumerable<DocumentoFI347StatusModel>>> ListarStatusMaterial(CancellationToken cancellationToken);
        Task<TryException<Return>> RegistroHistoricoProrogacao(SolicitacaoCienciaModel solicitacaoCienciaModel, CancellationToken cancellationToken);
    }
}
