using ICE.GDocs.Domain.Core.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento
{
    public interface IPassoUsuarioRepository : IRepository
    {
        Task<TryException<IEnumerable<AssinaturaPassoItemModel>>> Salvar(IEnumerable<AssinaturaPassoItemModel> assinaturaPassoItems, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<AssinaturaPassoItemModel>>> AtualizaStatusFareiEnvio(int passoUsuarioId, bool status, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<AssinaturaPassoAssinanteRepresentanteModel>>> ListarAssinantesERepresentantesPorPadIdEStatusNaoIniciadoEmAndamento(long processoId, bool todosStatus, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<AssinaturaPassoAssinanteModel>>> ListarPorPadId(IEnumerable<int> processoAssinaturaDocumentoId, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<AssinaturaPassoAssinanteModel>>> ListarPorPadId(IEnumerable<int> processoAssinaturaDocumentoId, Guid usuarioAd, CancellationToken cancellationToken);
        Task AtribuirRepresentantes(IEnumerable<AssinaturaPassoAssinanteRepresentanteModel> passosList, CancellationToken cancellationToken);
        Task<TryException<Return>> AssinarRejeitar(Guid usuarioAdGuid, IEnumerable<int> listaDeprocessoAssinaturaDocumentoId, StatusAssinaturaDocumentoPassoUsuario status, string justificativaRejeicao, CancellationToken cancellationToken);
        Task<TryException<Return>> InativarUsuariosDosPassos(int processoAssinaturaDocumentoId, CancellationToken cancellationToken);
    }
}
