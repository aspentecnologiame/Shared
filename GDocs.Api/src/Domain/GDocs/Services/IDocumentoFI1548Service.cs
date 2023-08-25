using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.DocumentoFI1548.ViewModel;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.Services
{
    public interface IDocumentoFI1548Service : IDomainService
    {
        Task<TryException<DocumentoFI1548Model>> Adicionar(DocumentoFI1548Model documento, CancellationToken cancellationToken);
        Task<TryException<DocumentoFI1548Model>> Cancelar(DocumentoFI1548Model documento, UsuarioModel usuarioModel, bool permiteCancelarTodos, CancellationToken cancellationToken, byte[] novoArquivo = default);
        Task<TryException<int>> EnviarParaAssinatura(DocumentoEhAssinaturaPassosModel documentoEhAssinaturaPassosModel, string processoAssinaturaDocumentoOrigemNome, string titulo, string nomeDocumento, int categoriaId, CancellationToken cancellationToken);
        Task<TryException<DocumentoFI1548Model>> Liquidar(DocumentoFI1548Model documento, bool permiteLiquidar, byte[] arquivo, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<AssinaturaArquivoModel>>> ListarArquivosUploadPorPadId(int processoAssinaturaDocumentoId, CancellationToken cancellationToken, bool listarTodos = false);
        Task<TryException<Return>> InativarFluxoAssinatura(int processoAssinaturaDocumentoId, CancellationToken cancellationToken);
        Task<TryException<ProcessoAssinaturaDocumentoOrigemModel>> ListarPorProcessoAssinaturaDocumentoOrigemIdENome(string ProcessoAssinaturaDocumentoOrigemId, string ProcessoAssinaturaDocumentoOrigemNome, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<AssinaturaPassoItemModel>>> ObterPassosEhUsuariosPorId(int processoAssinaturaDocumentoId,string origemDocumento, CancellationToken cancellationToken);
        Task<TryException<int>> InserirSolicitacaoCiencia(DocumentoFI1548CienciaModel documentoFI1548CienciaModel, CancellationToken cancellationToken);
        Task<TryException<DocumentoFI1548Model>> AtualizarStatusDoDocumento(DocumentoFI1548Model documentoFI1548Model, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<ProcessoAssinaturaDocumentoModel>>> ListarCienciasPendentesDeAprovacaoPorUsuario(Guid activeDirectoryId, CancellationToken cancellationToken);
        Task<TryException<DocumentoFI1548CienciaModel>> ObterCienciaPorId(int idSolicitacaoCiencia, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<CienciaUsuarioAprovacaoModel>>> ListarObsUsuarioPorCiencia(int idSolicitacaoCiencia, CancellationToken cancellationToken);
        Task<TryException<SoclicitacaoCienciaAprovadoresModel>> ObterAprovadoresCienciaCancelamento(int documentoId, CancellationToken cancellationToken);
        Task<TryException<bool>> RegistroCienciaPorUsuario(DocumentoFI1548CienciaModel documentoFI1548CienciaModel, UsuarioModel usuarioModel, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<AssinaturaInformacoesModel>>> ObterReferenciaSubstitutoOrigemDocumento(long processoAssinaturaOrigem, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<ObterMotivoCancelamentoModel>>> ObterMotivoCancelamento(int documentoId, CancellationToken cancellationToken);
    }
}
