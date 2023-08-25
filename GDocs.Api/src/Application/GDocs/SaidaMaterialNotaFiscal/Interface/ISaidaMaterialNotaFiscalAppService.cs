using ICE.GDocs.Application.Core.Services;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ICE.GDocs.Infra.CrossCutting.Models;

namespace ICE.GDocs.Application.GDocs.SaidaMaterialNotaFiscal.Interface
{
    public interface ISaidaMaterialNotaFiscalAppService : IApplicationService
    {
        Task<TryException<SaidaMaterialNotaFiscalModel>> Inserir(SaidaMaterialNotaFiscalModel solicitacaoSaidaMaterialModel, CancellationToken cancellationToken);

        Task<TryException<IEnumerable<DropDownModel>>> ListarNatureza(CancellationToken cancellationToken);

        Task<TryException<IEnumerable<DropDownModel>>> ListarModalidadeFrete(CancellationToken cancellationToken);

        Task<TryException<IEnumerable<DropDownModel>>> ListarStatusMaterial(CancellationToken cancellationToken);

        Task<TryException<IEnumerable<SaidaMaterialNotaFiscalModel>>> ConsultaMaterialNotaFiscalPorFiltro(SaidaMaterialNotaFiscalFilterModel saidaMaterialNotaFiscalFiltroModel, CancellationToken cancellationToken);

        Task<TryException<SaidaMaterialNotaFiscalModel>> ObterMaterialNotaFiscalPorId(int id, CancellationToken cancellationToken);

        Task<TryException<Return>> SalvarArquivosUpload(IEnumerable<SaidaMaterialArquivoModel> saidaMaterialArquivoModel, string uploadBasePath, Guid usuarioLogado, CancellationToken cancellationToken);

        Task<TryException<IEnumerable<SaidaMaterialArquivoModel>>> ObterBase64DoPdf(int saidaMaterialNfId, CancellationToken cancellationToken);
        Task<TryException<Return>> CancelarOuSolicitarCancelamento(int saidaMaterialNfId, string motivo, CancellationToken cancellationToken);

        Task<TryException<Return>> EfetivarCancelamento(SaidaMaterialNotaFiscalModel solicitacaoSaidaMaterialModel, CancellationToken cancellationToken);


        Task<TryException<IEnumerable<SaidaMaterialNotaFiscalModel>>> PesquisarSaidaMateriaisNotaFiscalPorFiltro(SaidaMaterialNotaFiscalFilterModel filtro, CancellationToken cancellationToken);

        Task<TryException<SaidaMaterialNotaFiscalHistoricoResponseModel>> ObterHistoricoMaterialNf(int saidaMaterialNfId, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<DropDownModel>>> ListarStatusMaterialFiltro(UsuarioModel usuario, CancellationToken cancellationToken);



    }
}
