using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ICE.GDocs.Domain.Core.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal.Enums;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;

namespace ICE.GDocs.Domain.GDocs.Repositories.SaidaMaterialNotaFiscal
{
    public interface ISaidaMaterialNotaFiscalRepository : IRepository
    {
        Task<TryException<SaidaMaterialNotaFiscalModel>> Inserir(SaidaMaterialNotaFiscalModel solicitacaoSaidaMaterialModel, CancellationToken cancellationToken);
        Task<TryException<SaidaMaterialNotaFiscalModel>> InserirFornecedor(SaidaMaterialNotaFiscalModel saidaMaterialNotaFiscalModel, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<DropDownModel>>> ListarNatureza(CancellationToken cancellationToken);
        Task<TryException<IEnumerable<DropDownModel>>> ListarModalidadeFrete(CancellationToken cancellationToken);
        Task<TryException<IEnumerable<DropDownModel>>> ListarStatusMaterial(CancellationToken cancellationToken);
        Task<TryException<IEnumerable<SaidaMaterialNotaFiscalModel>>> ConsultaMaterialNotaFiscalPorFiltro(SaidaMaterialNotaFiscalFilterModel filtroConsulta, CancellationToken cancellationToken, bool buscarNfCancelada = false);
        Task<TryException<SaidaMaterialNotaFiscalModel>> ObterMaterialNotaFiscalPorId(int id, CancellationToken cancellationToken);
        Task<TryException<Return>> AtualizarStatusPendenteSaidaEhNumero(SaidaMaterialArquivoModel saidaMaterialArquivoModel, CancellationToken cancellationToken);
        Task<TryException<Return>> AtualizarStatus(SaidaMaterialNotaFiscalModel saidaMaterialNotaFiscalModel, SaidaMaterialNotaFiscalStatus status, CancellationToken cancellationToken, bool retornoParcial = false);
        Task<TryException<Return>> InserirMotivoCancelar(SaidaMaterialNotaFiscalModel saidaMaterialNotaFiscalModel, string Motivo, CancellationToken cancellationToken);

        Task<TryException<Return>> AtualizarPorIdStatus(int saidaMaterialNotaFiscal, SaidaMaterialNotaFiscalStatus status, CancellationToken cancellationToken, bool retornoParcial = false);
        Task<TryException<Return>> AtualizarDataStatus(SaidaMaterialNotaFiscalCienciaModel solicitacaoCienciaNfModel, SaidaMaterialNotaFiscalStatus novoStatus, CancellationToken cancellationToken);
        Task<TryException<Return>> RegistroHistoricoProrogacao(SaidaMaterialNotaFiscalCienciaModel solicitacaoCienciaModel, CancellationToken cancellationToken);

    }
}
