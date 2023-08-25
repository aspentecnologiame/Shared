using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.Services
{
    public interface IProcessoAssinaturaDocumentoService : IDomainService
    {
        Task<TryException<AssinaturaModel>> Salvar(AssinaturaModel assinatura, string uploadBasePath, List<ConfiguracaoCategoriaModel> configCategorias, CancellationToken cancellationToken);
        Task<TryException<AssinaturaInformacoesModel>> CancelarProcesso(AssinaturaInformacoesModel model, CancellationToken cancellationToken);
        Task<TryException<AssinaturaModel>> CriarBinariosEIncluirArquivos(AssinaturaModel assinatura, string uploadBasePath, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<AssinaturaInformacoesModel>>> AtualizarStatusProcessos(IEnumerable<int> processoListId, StatusAssinaturaDocumento status, CancellationToken cancellationToken);
        Task<TryException<Return>> AssinarRejeitar(Guid usuarioAdGuid, IEnumerable<int> listaDeprocessoAssinaturaDocumentoId, StatusAssinaturaDocumentoPassoUsuario status, string justificativaRejeicao, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<AssinaturaPassoAssinanteModel>>> ListarPorPadId(IEnumerable<int> processoAssinaturaDocumentoId, CancellationToken cancellationToken);
        Task<TryException<Return>> InativarPassos(int processoAssinaturaDocumentoId, CancellationToken cancellationToken);
        Task<TryException<Return>> InativarUsuariosDosPassos(int processoAssinaturaDocumentoId, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<AssinaturaPassoItemModel>>> ObterPassosEhUsuariosPorId(int processoAssinaturaDocumentoId, CancellationToken cancellationToken);
    }
}
