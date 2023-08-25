using ICE.GDocs.Application.Core.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.DocumentoFI1548.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Application.GDocs
{
    public interface IDocumentoFI1548AppService : IApplicationService
    {
        Task<TryException<DocumentoFI1548Model>> Adicionar(DocumentoFI1548Model documento, UsuarioModel usuarioModel, CancellationToken cancellationToken);
        Task<TryException<DocumentoFI1548Model>> Cancelar(DocumentoFI1548Model documento, UsuarioModel usuarioModel, bool permiteCancelarTodos, CancellationToken cancellationToken);
        Task<TryException<DocumentoFI1548Model>> CancelarOuSolicitarCiencia(int documentoId, string motivo, UsuarioModel usuarioModel, bool permiteCancelarTodos, CancellationToken cancellationToken);
        Task<TryException<DocumentoFI1548Model>> Liquidar(DocumentoFI1548Model model, bool permiteLiquidar, string uploadPathBase, CancellationToken cancellationToken);
        Task<TryException<DocumentoFI1548Model>> ObterPorId(int id, CancellationToken cancellationToken);
        Task<TryException<DocumentoFI1548Model>> ObterPorNumero(int numero, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<DocumentoFI1548Model>>> ObterPorNumeroTipoPagamentoAutorPeriodo(DocumentoFI1548FilterModel filtro, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<DocumentoFI1548StatusModel>>> ListarStatusPagamento(CancellationToken cancellationToken);
        Task<TryException<IEnumerable<AssinaturaArquivoModel>>> EnviarParaAssinatura(DocumentoEhAssinaturaPassosModel documentoEhAssinaturaPassosModel, CancellationToken cancellationToken);
        Task<TryException<string>> ObterBase64DoPdf(int documentoId, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<AssinaturaPassoItemModel>>> ObterPassosEhUsuariosPorId(int documentoId, CancellationToken cancellationToken);
        Task<TryException<ProcessoAssinaturaDocumentoOrigemModel>> ObterOrigemDocumento(int documentoId, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<DropDownCustonModel>>> ListarReferenciaSubstitutos(Guid usuarioLogadoAd, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<ProcessoAssinaturaDocumentoModel>>> ListarCienciasPendentesDeAprovacaoPorUsuario(Guid activeDirectoryId, CancellationToken cancellationToken);
        Task<TryException<DocumentoFI1548CienciaModel>> ObterCienciaPorId(int idSolicitacaoCiencia, CancellationToken cancellationToken);
        Task<TryException<SoclicitacaoCienciaAprovadoresModel>> ObterAprovadoresCienciaCancelamento(int documentoId, CancellationToken cancellationToken);
        Task<TryException<Return>> RegistroCienciaPorUsuario(DocumentoFI1548CienciaModel documentoFI1548CienciaModel, UsuarioModel usuarioModel, CancellationToken cancellationToken);

        Task<TryException<IEnumerable<AssinaturaInformacoesModel>>> ObterReferenciaSubstitutoOrigemDocumento(long documentoSubstitutoId, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<ObterMotivoCancelamentoModel>>> ObterMotivoCancelamento(int documentoId, CancellationToken cancellationToken);
    }
    
}
