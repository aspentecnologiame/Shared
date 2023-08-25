using ICE.GDocs.Application.Core.Services;
using ICE.GDocs.Domain.ExternalServices.Model;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Application.GDocs
{
    public interface IAssinaturaAppService : IApplicationService
    {
        Task<TryException<ArquivoModel>> Converter(string extensao, ArquivoModel arquivo, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<UsuarioActiveDirectory>>> ListarUsuariosActiveDirectory(string nome, CancellationToken cancellationToken);
        Task<TryException<AssinaturaModel>> SalvarProcesso(AssinaturaModel assinatura, string uploadBasePath, CancellationToken cancellationToken);
        Task<TryException<AssinaturaInformacoesModel>> CancelarProcesso(AssinaturaInformacoesModel model, CancellationToken cancellationToken, DocumentoFI1548FilterModel filtro, UsuarioModel usuario, bool permiteCancelar);
        Task<TryException<IEnumerable<AssinaturaInformacoesModel>>> ObterInformacoesPorNumeroNomeStatusAutorPeriodo(AssinaturaInformacoesFilterModel filtro, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<AssinaturaNomeDocumento>>> ListarNomesDocumentos(string termo, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<AssinaturaPassoAssinanteRepresentanteModel>>> ListarAssinantesProcesso(long processoId, bool todosStatus, CancellationToken cancellationToken);
        Task<TryException<AssinaturaModel>> ObterProcessoComPassosParaAssinatura(long processoId, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<AssinaturaPassoAssinanteRepresentanteModel>>> AtribuirRepresentantes(IEnumerable<AssinaturaPassoAssinanteRepresentanteModel> passosList, CancellationToken cancellationToken);
        Task<TryException<Return>> SalvarArquivos(IEnumerable<AssinaturaArquivoModel> assinaturaArquivoModel, string uploadBasePath, CancellationToken cancellationToken);
        Task<TryException<Return>> Assinar(Guid usuarioAdGuid, AssinaturaPassoItemAssinarRejeitarModel assinaturaPassoItemAssinarModel, string uploadPathBase, CancellationToken cancellationToken);
        Task<TryException<Return>> Rejeitar(Guid usuarioAdGuid, AssinaturaPassoItemAssinarRejeitarModel assinaturaPassoItemRejeitarModel, CancellationToken cancellationToken);
        Task<TryException<Return>> ValidarSeOUsuarioJaAssinouAlgumDosProcessos(Guid usuarioAdGuid, IEnumerable<int> listaDeprocessoAssinaturaDocumentoId, StatusAssinaturaDocumentoPassoUsuario status, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<AssinaturaArquivoModel>>> ListarArquivosUploadPorPadId(int processoAssinaturaDocumentoId, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<ProcessoAssinaturaDocumentoModel>>> ListarDocumentosPendentesDeAssinaturaOuJaAssinadoPeloUsuario(Guid activeDirectoryId, CancellationToken cancellationToken);
        Task<TryException<Return>> AtualizarDestaquePorPadId(int processoAssinaturaDocumentoId, bool destaque, CancellationToken cancellationToken);
        Task<TryException<AssinaturaInformacoesModel>> ObterInformacaoPorId(int processoAssinaturaDocumentoId, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<AssinaturaPassoItemModel>>> ListarPassosEUsuariosPorTag(string tag, CancellationToken cancellationToken);
        Task<TryException<Return>> SalvarFareiEnvio(AssinaturaModel assinaturaModel, Guid usuarioId, bool fareiEnvio , CancellationToken cancellationToken);
        Task<TryException<bool>> VerificarProcessoCompleto(int processoId, Guid usuarioId , CancellationToken cancellationToken);
        Task<TryException<DocumentoFI1548Model>> ObterDocumentoFI1548PorNumero(DocumentoFI1548FilterModel documento, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<AssinaturaArquivoModel>>> ListarArquivoExpurgoPorPad(int processoAssinaturaDocumentoId, CancellationToken cancellationToken, bool listarTodos = false);
    }
}