using ICE.GDocs.Application.Core.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Application.GDocs.SolicitacaoSaidaMaterial
{
    public interface ISolicitacaoSaidaMaterialAppService : IApplicationService
    {
        Task<TryException<SolicitacaoSaidaMaterialModel>> ObterPorId(int id, CancellationToken cancellationToken);
        Task<TryException<SolicitacaoSaidaMaterialModel>> Inserir(SolicitacaoSaidaMaterialModel solicitacaoSaidaMaterialModel, CancellationToken cancellationToken);
        Task<TryException<SolicitacaoSaidaMaterialModel>> Atualizar(SolicitacaoSaidaMaterialModel solicitacaoSaidaMaterialModel, CancellationToken cancellationToken);
        Task<TryException<Return>> Excluir(int id, CancellationToken cancellationToken);
        Task<TryException<int>> EnviarParaAssinatura(SolicitacaoSaidaMaterialModel documento, CancellationToken cancellationToken);
        Task<TryException<SolicitacaoSaidaMaterialModel>> ObterMaterialComItensPorId(int id, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<SolicitacaoSaidaMaterialModel>>> ConsultarSolicitacoesDeSaidaMateriaisPorFiltro(SolicitacaoSaidaMaterialFilterModel filtro, CancellationToken cancellationToken);
        Task<TryException<string>> ObterBase64DoPdf(int saidaMaterialId, CancellationToken cancellationToken);
        Task<TryException<SolicitacaoSaidaMaterialModel>> CancelarOuSolicitarCiencia(int solicitacaoSaidaMaterialId, string motivo, UsuarioModel usuarioModel, bool permiteCancelarTodos, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<DocumentoFI347StatusModel>>> ListarStatusMaterial(CancellationToken cancellationToken);
    }
}
