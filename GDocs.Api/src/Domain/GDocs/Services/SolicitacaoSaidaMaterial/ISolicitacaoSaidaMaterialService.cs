using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.GDocs.Services.SolicitacaoSaidaMaterial
{
    public interface ISolicitacaoSaidaMaterialService : IDomainService
    {
        Task<TryException<SolicitacaoSaidaMaterialModel>> ObterPorId(int id, CancellationToken cancellationToken);
        Task<TryException<SolicitacaoSaidaMaterialModel>> ObterMaterialComItensPorId(int id, CancellationToken cancellationToken);
        
        Task<TryException<SolicitacaoSaidaMaterialModel>> Inserir(SolicitacaoSaidaMaterialModel solicitacaoSaidaMaterialModel, CancellationToken cancellationToken);
        Task<TryException<SolicitacaoSaidaMaterialModel>> Atualizar(SolicitacaoSaidaMaterialModel solicitacaoSaidaMaterialModel, CancellationToken cancellationToken);
        Task<TryException<Return>> Excluir(int id, CancellationToken cancellationToken);
        Task<TryException<SolicitacaoSaidaMaterialModel>> Adicionar(SolicitacaoSaidaMaterialModel solicitacaoSaidaMaterialModel, CancellationToken cancellationToken);
        Task<TryException<byte[]>> ObterRelatorio(SolicitacaoSaidaMaterialModel solicitacaoSaidaMaterialModel, CancellationToken cancellationToken);
        Task<TryException<int>> EnviarParaAssinatura(SolicitacaoSaidaMaterialModel documento, string processoAssinaturaDocumentoOrigemNome, string titulo, string nomeDocumento, int categoriaId, CancellationToken cancellationToken);
        Task<TryException<SolicitacaoSaidaMaterialModel>> Cancelar(SolicitacaoSaidaMaterialModel saidaMaterial, UsuarioModel usuarioModel, bool permiteCancelarTodos, CancellationToken cancellationToken, byte[] novoArquivo = default);
    }
}
