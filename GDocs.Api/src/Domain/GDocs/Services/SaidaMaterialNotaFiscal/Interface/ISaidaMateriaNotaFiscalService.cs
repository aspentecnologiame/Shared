using ICE.GDocs.Domain.Core.Services;
using System;
using System.Threading.Tasks;
using System.Threading;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using ICE.GDocs.Infra.CrossCutting.Models;
using System.Collections.Generic;

namespace ICE.GDocs.Domain.GDocs.Services.SaidaMaterialNotaFiscal.Interface
{
    public interface ISaidaMateriaNotaFiscalService : IDomainService
    {
        Task<TryException<SaidaMaterialNotaFiscalModel>> Inserir(SaidaMaterialNotaFiscalModel saidaMaterialNotaFiscalModel, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<DropDownModel>>> ListarNatureza(CancellationToken cancellationToken);
        Task<TryException<IEnumerable<DropDownModel>>> ListarModalidadeFrete(CancellationToken cancellationToken);
        Task<TryException<IEnumerable<DropDownModel>>> ListarStatusMaterial(CancellationToken cancellationToken);
        Task<TryException<IEnumerable<SaidaMaterialNotaFiscalModel>>> ConsultaMaterialNotaFiscalPorFiltro(SaidaMaterialNotaFiscalFilterModel saidaMaterialNotaFiscalFiltroModel, CancellationToken cancellationToken, bool buscarNfCancelada = false);
        Task<TryException<SaidaMaterialNotaFiscalModel>> ObterMaterialNotaFiscalPorId(int id, CancellationToken cancellationToken);
        Task<TryException<Return>> SalvarArquivosUploadSaida(IEnumerable<SaidaMaterialArquivoModel> saidaMaterialArquivoModel, string uploadBasePath, Guid usuarioLogado, CancellationToken cancellationToken);
        Task<TryException<Return>> SalvarArquivosUploadRetorno(SaidaMaterialNotaFiscalModel saidaMaterialNotaFiscalModel, IEnumerable<SaidaMaterialArquivoModel> saidaMaterialArquivoModel, string uploadBasePath, Guid usuarioLogado, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<SaidaMaterialArquivoModel>>> ObterBase64DoPdf(int saidaMaterialNfId, CancellationToken cancellationToken);
        Task<TryException<Return>> CancelarOuSolicitarCancelamento(SaidaMaterialNotaFiscalModel saidaMaterialNotaFiscalModel, string motivo, CancellationToken cancellationToken);
        Task<TryException<Return>> EfetivarCancelamento(SaidaMaterialNotaFiscalModel saidaMaterialNotaFiscalModel, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<DropDownModel>>> ListarStatusMaterialFiltro(UsuarioModel usuario, CancellationToken cancellationToken);


    }
}
