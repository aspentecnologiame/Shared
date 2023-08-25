using ICE.GDocs.Domain.Core.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.DocumentoFI1548.ViewModel;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.Repositories
{
    public interface IDocumentoFI1548Repository : IRepository
    {
        Task<TryException<DocumentoFI1548Model>> Salvar(DocumentoFI1548Model docFI1548, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<DocumentoFI1548Model>>> Listar(DocumentoFI1548FilterModel filtro, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<DocumentoFI1548StatusModel>>> ListarStatusPagamento(CancellationToken cancellationToken);
        Task<TryException<IEnumerable<DropDownCustonModel>>> ListarSubstitutoRejeitadoAssinatura(Guid UsuarioLogadoAd, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<DropDownCustonModel>>> ListarSubstitutoAguardandoCiencia(Guid UsuarioLogadoAd, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<DropDownCustonModel>>> ListarSubstitutoAprovadosPorCiencia(Guid UsuarioLogadoAd, CancellationToken cancellationToken);
        Task<TryException<int>> Inserir(DocumentoFI1548CienciaModel documentoFI1548CienciaModel, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<ProcessoAssinaturaDocumentoModel>>> ListarCienciasPendentesDeAprovacaoPorUsuario(Guid activeDirectoryId, CancellationToken cancellationToken);
        Task<TryException<DocumentoFI1548CienciaModel>> ObterCienciaPorId(int idSolicitacaoCiencia, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<CienciaUsuarioAprovacaoModel>>> ListarObsUsuarioPorCiencia(int idSolicitacaoCiencia, CancellationToken cancellationToken);
        Task<TryException<SoclicitacaoCienciaAprovadoresModel>> ObterAprovadoresCienciaCancelamento(int documentoId, CancellationToken cancellationToken);
        Task<TryException<CienciaUsuariosProvacao>> ObterAprovadoresPorUsuarioECiencia(int idSolicitacaoCiencia, Guid usuarioId, CancellationToken cancellationToken);
        Task<TryException<int>> RegistroCiencia(DocumentoFI1548CienciaModel documentoFI1548CienciaModel, int idCienciaAprovador, CancellationToken cancellationToken);
        Task<TryException<int>> AtualizarCienciaStatus(DocumentoFI1548CienciaModel documentoFI1548CienciaModel, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<CienciaUsuariosProvacao>>> ObterAprovadoresPorCiencia(int idSolicitacaoCiencia, CancellationToken cancellationToken);
        Task<TryException<Return>> AtualizarStatus(int documentoFI1548Id, DocumentoFI1548Status novoStatus, CancellationToken cancellationToken);
        Task<TryException<int>> ObterReferenciaSubstituto(long processoAssinaturaOrigem, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<ObterMotivoCancelamentoModel>>> ObterMotivoCancelamento(int documentoId, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<ObterMotivoCancelamentoModel>>> ObterJustificativaReprovacao(int documentoId, CancellationToken cancellationToken);
        Task<TryException<DocumentoFI1548Model>> ObterStatusPagamentoPorId(int documentoId, CancellationToken cancellationToken);
    }
}
