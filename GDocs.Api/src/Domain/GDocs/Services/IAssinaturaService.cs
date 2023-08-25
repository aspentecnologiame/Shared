using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.Services
{
    public interface IAssinaturaService : IDomainService
    {
        Task<TryException<ArquivoModel>> Converter(string extensao, ArquivoModel arquivo, CancellationToken cancellationToken);
        Task<TryException<AssinaturaModel>> SalvarProcesso(AssinaturaModel assinatura, string uploadBasePath, CancellationToken cancellationToken);
        Task<TryException<AssinaturaInformacoesModel>> CancelarProcesso(AssinaturaInformacoesModel model, CancellationToken cancellationToken);
        Task<TryException<Return>> SalvarFareiEnvio(AssinaturaModel assinaturaModel, Guid usuarioId, bool fareiEnvio, CancellationToken cancellationToken);
        Task<TryException<Return>> SalvarArquivos(IEnumerable<AssinaturaArquivoModel> assinaturaArquivoModel, string uploadBasePath, CancellationToken cancellationToken);
        Task<TryException<Return>> Assinar(Guid usuarioAdGuid, AssinaturaPassoItemAssinarRejeitarModel assinaturaPassoItemAssinarModel, string uploadPathBase, CancellationToken cancellationToken);
        Task<TryException<Return>> Rejeitar(Guid usuarioAdGuid, AssinaturaPassoItemAssinarRejeitarModel assinaturaPassoItemRejeitarModel, CancellationToken cancellationToken);
        Task<TryException<Return>> ValidarSeOUsuarioJaAssinouAlgumDosProcessos(Guid usuarioAdGuid, IEnumerable<int> listaDeprocessoAssinaturaDocumentoId, StatusAssinaturaDocumentoPassoUsuario status, CancellationToken cancellationToken);
        Task<TryException<Return>> AtualizarDestaqueProcesso(AssinaturaInformacoesModel assinaturaInformacoesModel, CancellationToken cancellationToken);
    }
}
